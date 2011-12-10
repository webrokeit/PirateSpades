namespace PirateSpades.GameLogic {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics.Contracts;

    public class Round {
        public Game Game { get; private set; }
        public int Cards { get; private set; }
        public int TotalCards {
            get {
                return Game.Players.Count * Cards;
            }
        }
        public int CardsDealt {
            get {
                return AwaitingBets ? Game.Players.Sum(player => player.CardsOnHand) : TotalCards;
            }
        }
        public bool AwaitingBets { get; private set; }
        public bool BetsDone {
            get {
                return PlayerBets.Count(bet => bet.Value > -1) == Game.Players.Count;
            }
        }
        public bool Finished {
            get {
                return TricksDone >= Cards;
            }
        }
        private bool PileDone {
            get {
                return BoardCards.Pile.Count == Game.Players.Count;
            }
        }

        public int Dealer { get; private set; }
        public int CurrentPlayer { get; private set; }

        public Trick BoardCards { get; private set; }
        public Trick LastTrick { get; private set; }
        private int TricksDone { get; set; }

        public Dictionary<Player, List<Trick>> PlayerTricks { get; private set; }
        public Dictionary<Player, int> PlayerBets { get; private set; }

        public delegate void RoundEventDelegate(Round round);

        public event RoundEventDelegate NewPile;
        public event RoundEventDelegate RoundStarted;
        public event RoundEventDelegate RoundBegun;
        public event RoundEventDelegate RoundFinished;

        public Round(Game game, int dealer) {
            Contract.Requires(game != null && dealer >= 0 && dealer < game.Players.Count);
            this.Game = game;
            this.Cards = Card.CardsToDeal(Game.CurrentRound, Game.Players.Count);
            this.Dealer = dealer;
            this.CurrentPlayer = dealer;
            this.BoardCards = new Trick();
            this.PlayerTricks = new Dictionary<Player, List<Trick>>();
            this.PlayerBets = new Dictionary<Player, int>();
            TricksDone = 0;
        }

        public void Start() {
            Contract.Requires(!Finished && !AwaitingBets && Game.Players.Count >= Game.MinPlayersInGame);
            Contract.Ensures(AwaitingBets);
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

        public void Begin() {
            Contract.Requires(!Finished && BetsDone && AwaitingBets);
            Contract.Ensures(!AwaitingBets);
            AwaitingBets = false;
            if(RoundBegun != null) RoundBegun(this);
        }

        private void Finish() {
            Contract.Requires(Finished);
            if(RoundFinished != null) RoundFinished(this);
        }

        public void PlayerBet(Player player, int bet) {
            Contract.Requires(player != null && bet >= 0 && bet <= Cards && AwaitingBets && PlayerBets.ContainsKey(player));
            PlayerBets[player] = bet;
        }

        public void PlayerBet(string playerName, int bet) {
            Contract.Requires(playerName != null);
            this.PlayerBet(Game.GetPlayer(playerName), bet);
        }

        public void PlayCard(Player player, Card card) {
            Contract.Requires(player != null && card != null && player.HasCard(card));
            BoardCards.PlaceCard(player, card);
            if(PileDone) {
                this.NextPile();
            } else {
               this.NextPlayer(); 
            }
        }

        private void NextPlayer() {
            CurrentPlayer = (CurrentPlayer + 1) % Game.Players.Count;
        }

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

        public int PlayerScore(Player player) {
            Contract.Requires(player != null && PlayerBets.ContainsKey(player) && PlayerTricks.ContainsKey(player));
            var bet = PlayerBets[player];
            var tricks = PlayerTricks[player].Count;
            return bet == tricks ? 10 + bet : -Math.Abs(bet - tricks);
        }

        public Dictionary<Player, int> PlayerScores() {
            return this.Game.Players.ToDictionary(p => p, this.PlayerScore);
        }

        public static int RoundsPossible(int players) {
            Contract.Requires(players > 0);
            return (52 / players < 10 ? 52 / players : 10) * 2;
        }
    }
}
