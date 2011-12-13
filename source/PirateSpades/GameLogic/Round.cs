// <copyright file="Round.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A representation of a round in the Pirate Spades Game.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.GameLogic {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A representation of a round in the Pirate Spades Game.
    /// </summary>
    public class Round {
        /// <summary>
        /// The game currently being played
        /// </summary>
        public Game Game { get; private set; }

        /// <summary>
        /// The round number
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// The number of cards this round
        /// </summary>
        public int Cards { get; private set; }

        /// <summary>
        /// The total number of cards in play this round
        /// </summary>
        public int TotalCards {
            get {
                return Game.Players.Count * Cards;
            }
        }

        /// <summary>
        /// How many cards have been deal this round?
        /// </summary>
        public int CardsDealt {
            get {
                return AwaitingBets ? Game.Players.Sum(player => player.CardsOnHand) : TotalCards;
            }
        }

        /// <summary>
        /// Is the round started?
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        /// Are we still awaiting bets from players?
        /// </summary>
        public bool AwaitingBets { get; private set; }

        /// <summary>
        /// Have all players bet this round?
        /// </summary>
        public bool BetsDone {
            get {
                return PlayerBets.Count(bet => bet.Value > -1) == Game.Players.Count;
            }
        }

        /// <summary>
        /// Is the round finished?
        /// </summary>
        public bool Finished {
            get {
                return Started && TricksDone >= Cards;
            }
        }

        /// <summary>
        /// Has all players played a card?
        /// </summary>
        private bool PileDone {
            get {
                return BoardCards.Pile.Count == Game.Players.Count;
            }
        }

        /// <summary>
        /// Who is the dealer?
        /// </summary>
        public int Dealer { get; private set; }

        /// <summary>
        /// Whose turn is it to play a card?
        /// </summary>
        public int CurrentPlayer { get; private set; }

        /// <summary>
        /// What cards are on the table?
        /// </summary>
        public Trick BoardCards { get; private set; }

        /// <summary>
        /// What was the last trick?
        /// </summary>
        public Trick LastTrick { get; private set; }

        /// <summary>
        /// How many tricks have been played this round?
        /// </summary>
        private int TricksDone { get; set; }

        /// <summary>
        /// May I have a dictionary describing the players' won tricks?
        /// </summary>
        public Dictionary<Player, List<Trick>> PlayerTricks { get; private set; }

        /// <summary>
        /// May I have a dictionary describing what the players have bet this round?
        /// </summary>
        public Dictionary<Player, int> PlayerBets { get; private set; }

        /// <summary>
        /// Delegate to be used for events involving Round
        /// </summary>
        /// <param name="round">Round</param>
        public delegate void RoundEventDelegate(Round round);

        /// <summary>
        /// Fires when a pile done
        /// </summary>
        public event RoundEventDelegate NewPile;

        /// <summary>
        /// Fires when a round is started
        /// </summary>
        public event RoundEventDelegate RoundStarted;

        /// <summary>
        /// Fires when a round hass begun
        /// </summary>
        public event RoundEventDelegate RoundBegun;

        /// <summary>
        /// Fires when a round is finished
        /// </summary>
        public event RoundEventDelegate RoundFinished;

        /// <summary>
        /// Make a new round with this game, number and dealer
        /// </summary>
        /// <param name="game">The game</param>
        /// <param name="number">The roundnumber</param>
        /// <param name="dealer">The dealer</param>
        public Round(Game game, int number, int dealer) {
            Contract.Requires(game != null && dealer >= 0 && dealer < game.Players.Count);
            this.Game = game;
            this.Dealer = dealer;
            this.Number = number;
            this.CurrentPlayer = dealer;
            this.BoardCards = new Trick();
            this.PlayerTricks = new Dictionary<Player, List<Trick>>();
            this.PlayerBets = new Dictionary<Player, int>();
            this.Started = false;
            this.TricksDone = 0;
            this.Cards = 0;
        }

        /// <summary>
        /// Start this round
        /// </summary>
        public void Start() {
            Contract.Requires(!Finished && !AwaitingBets && Game.Players.Count >= Game.MinPlayersInGame);
            Contract.Ensures(AwaitingBets);
            this.Started = true;
            this.Cards = Card.CardsToDeal(Number, Game.Players.Count);
            foreach(var player in Game.Players) {
                player.IsDealer = false;
                PlayerTricks.Add(player, new List<Trick>());
                PlayerBets.Add(player, -1);
            }

            Game.Players[this.Dealer].IsDealer = true;
            AwaitingBets = true;
            this.NextPlayer();
            if(RoundStarted != null) RoundStarted(this);
        }

        /// <summary>
        /// Begin this round
        /// </summary>
        public void Begin() {
            Contract.Requires(!Finished && BetsDone && AwaitingBets);
            Contract.Ensures(!AwaitingBets);
            AwaitingBets = false;
            if(RoundBegun != null) RoundBegun(this);
        }

        /// <summary>
        /// Finish this round
        /// </summary>
        private void Finish() {
            Contract.Requires(Finished);
            if(RoundFinished != null) RoundFinished(this);
        }

        /// <summary>
        /// This player has bet this
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="bet">The bet</param>
        public void PlayerBet(Player player, int bet) {
            Contract.Requires(player != null && bet >= 0 && bet <= Cards && AwaitingBets && PlayerBets.ContainsKey(player));
            PlayerBets[player] = bet;
        }

        /// <summary>
        /// This player has bet this
        /// </summary>
        /// <param name="playerName">The player</param>
        /// <param name="bet">The bet</param>
        public void PlayerBet(string playerName, int bet) {
            Contract.Requires(playerName != null);
            this.PlayerBet(Game.GetPlayer(playerName), bet);
        }

        /// <summary>
        /// Has this player bet?
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The bet of the player</returns>
        public bool HasPet(Player player) {
            Contract.Requires(player != null && PlayerBets.ContainsKey(player));
            return PlayerBets[player] > -1;
        }

        /// <summary>
        /// This player has played this card
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="card">The card</param>
        public void PlayCard(Player player, Card card) {
            Contract.Requires(player != null && card != null && player.HasCard(card));
            BoardCards.PlaceCard(player, card);
            if(PileDone) {
                this.NextPile();
            } else {
               this.NextPlayer(); 
            }
        }

        /// <summary>
        /// Switch the player turn
        /// </summary>
        private void NextPlayer() {
            CurrentPlayer = (CurrentPlayer + 1) % Game.Players.Count;
        }

        /// <summary>
        /// Make a new pile and remove the old one
        /// </summary>
        private void NextPile() {
            var winner = BoardCards.Winner;
            PlayerTricks[winner].Add(BoardCards);
            CurrentPlayer = Game.PlayerIndex(winner);
            LastTrick = BoardCards;
            BoardCards = new Trick();

            if (NewPile != null) NewPile(this);

            TricksDone++;
            if(Finished) {
                this.Finish();
            }
        }

        /// <summary>
        /// How much points did this player get this round?
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The points to be given to the player</returns>
        public int PlayerScore(Player player) {
            Contract.Requires(player != null && PlayerBets.ContainsKey(player) && PlayerTricks.ContainsKey(player));
            var bet = PlayerBets[player];
            var tricks = PlayerTricks[player].Count;
            return bet == tricks ? 10 + bet : -Math.Abs(bet - tricks);
        }

        /// <summary>
        /// May I have a dictionary containing with all the players and how many points they got this round?
        /// </summary>
        /// <returns></returns>
        public Dictionary<Player, int> PlayerScores() {
            return this.Game.Players.ToDictionary(p => p, this.PlayerScore);
        }

        /// <summary>
        /// How many rounds are possible?
        /// </summary>
        /// <param name="players">The amount of players</param>
        /// <returns>The amount of rounds possible with this amount of players</returns>
        public static int RoundsPossible(int players) {
            Contract.Requires(players > 0);
            return (52 / players < 10 ? 52 / players : 10) * 2;
        }
    }
}
