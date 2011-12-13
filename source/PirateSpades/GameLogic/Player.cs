// <copyright file="Player.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A representation of a player in the Pirate Spades Game.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.GameLogic {
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// A representation of a player in the Pirate Spades Game.
    /// </summary>
    public class Player {
        /// <summary>
        /// The game the player is playing in.
        /// </summary>
        public Game Game { get; private set; }

        /// <summary>
        /// The name of the player.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The amount the player bet.
        /// </summary>
        public int Bet { get; private set; }

        /// <summary>
        /// Whether or not the player is the current dealer.
        /// </summary>
        public bool IsDealer { get; set; }

        /// <summary>
        /// The card the player wishes to play.
        /// </summary>
        public Card CardToPlay { get; private set; }

        /// <summary>
        /// The hand of the player.
        /// </summary>
        public IList<Card> Hand { get; protected set; }

        /// <summary>
        /// The cards on hand of the player, sorted 2, 3 -> King, Ace and Diamonds, Clubs, Hearts, Spades.
        /// </summary>
        protected SortedSet<Card> Cards { get; set; }

        /// <summary>
        /// Amount of cards on hand.
        /// </summary>
        public int CardsOnHand {
            get {
                return Hand.Count;
            }
        }

        /// <summary>
        /// Amount of tricks in the current round.
        /// </summary>
        public int Tricks {
            get {
                return Game != null && Game.Started && Game.CurrentRound >= 1 && Game.GetRound(Game.CurrentRound).Started && Game.Round.PlayerTricks.ContainsKey(this)
                           ? Game.Round.PlayerTricks[this].Count
                           : 0;
            }
        }

        /// <summary>
        /// Delegate for when a card has been played.
        /// </summary>
        /// <param name="c">The card played.</param>
        protected delegate void CardPlayedDelegate(Card c);

        /// <summary>
        /// Delegate for when a card has been dealt.
        /// </summary>
        /// <param name="p">The receiving players.</param>
        /// <param name="c">The card dealt.</param>
        protected delegate void CardDealtDelegate(Player p, Card c);

        /// <summary>
        /// Delegate for when a bet has been set.
        /// </summary>
        /// <param name="bet">The bet set.</param>
        protected delegate void BetSetDelegate(int bet);

        /// <summary>
        /// Fires when a card has been played.
        /// </summary>
        protected event CardPlayedDelegate CardPlayed;

        /// <summary>
        /// Fires when a card has been dealt.
        /// </summary>
        protected event CardDealtDelegate CardDealt;

        /// <summary>
        /// Fires when a bet has been set.
        /// </summary>
        protected event BetSetDelegate BetSet;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        public Player(string name) {
            Contract.Requires(name != null);
            this.Name = name;
            this.Cards = new SortedSet<Card>();
            this.Hand = Cards.ToList().AsReadOnly();
            this.IsDealer = false;
            this.UpdateHand();
        }

        /// <summary>
        /// Update the hand.
        /// </summary>
        private void UpdateHand() {
            lock (this.Cards) {
                this.Hand = this.Cards.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Receive a card.
        /// </summary>
        /// <param name="card">The card to receive.</param>
        public void GetCard(Card card) {
            Contract.Requires(card != null);
            lock (this.Cards) {
                this.Cards.Add(card);
            }
            this.UpdateHand();
        }

        /// <summary>
        /// Remove card from hand.
        /// </summary>
        /// <param name="card">The card to remove.</param>
        public void RemoveCard(Card card) {
            Contract.Requires(card != null && this.Cards.Contains(card));
            lock (this.Cards) {
                this.Cards.Remove(card);
            }
            this.UpdateHand();
        }

        /// <summary>
        /// Clear the cards on hand.
        /// </summary>
        public void ClearHand() {
            this.Cards.Clear();
            this.UpdateHand();
        }

        /// <summary>
        /// Play a card.
        /// </summary>
        /// <param name="card">The card to play.</param>
        public void PlayCard(Card card) {
            Contract.Requires(card != null && this.HasCard(card) && this.CardPlayable(card, Game.Round.BoardCards.FirstCard));
            Contract.Ensures(!this.HasCard(card));
            CardToPlay = card;
            Game.Round.PlayCard(this, card);
            this.RemoveCard(card);
            if(CardPlayed != null) CardPlayed(card);
        }

        /// <summary>
        /// Deal cards.
        /// </summary>
        public void DealCards() {
            Contract.Requires(IsDealer && Game != null);
            var deck = Deck.GetShuffledDeck();
            var dealTo = (Game.Round.Dealer + 1) % Game.Players.Count;
            for(var i = 0; i < Game.CardsToDeal * Game.Players.Count; i++) {
                var card = deck.Pop();
                Game.Players[dealTo].GetCard(card);
                if (CardDealt != null) CardDealt(Game.Players[dealTo], card);
                dealTo = (dealTo + 1) % Game.Players.Count;
            }
        }

        /// <summary>
        /// Checks whether or not a card is playable.
        /// </summary>
        /// <param name="toPlay">The card to play.</param>
        /// <returns>True if the card is playable, false if not.</returns>
        [Pure]
        public bool CardPlayable(Card toPlay) {
            Contract.Requires(toPlay != null);
            Contract.Requires(this.HasCard(toPlay));
            Contract.Requires(this.Game.Started && this.Game.Round != null);
            return CardPlayable(toPlay, Game.Round.BoardCards.FirstCard);
        }

        /// <summary>
        /// Checks whether or not a card is playable.
        /// </summary>
        /// <param name="toPlay">The card to play.</param>
        /// <param name="mustMatch">The card it must match.</param>
        /// <returns>True if the card is playable, false if not.</returns>
        [Pure]
        public bool CardPlayable(Card toPlay, Card mustMatch) {
            Contract.Requires(toPlay != null);
            Contract.Requires(this.HasCard(toPlay));
            return mustMatch == null || (!this.HasCardOf(mustMatch.Suit) || toPlay.SameSuit(mustMatch));
        }

        /// <summary>
        /// Checks whether or not the player has a card of the specified suit.
        /// </summary>
        /// <param name="suit">The suit to check for.</param>
        /// <returns>True if the player has a card of the specified suit, false if not.</returns>
        [Pure]
        public bool HasCardOf(Suit suit) {
            return Hand.Any(c => c.Suit == suit);
        }

        /// <summary>
        /// Checks whether or not the player has the specified card.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if the player has the specified card.</returns>
        [Pure]
        public bool HasCard(Card card) {
            lock (Cards) {
                return this.Cards.Contains(card);
            }
        }

        /// <summary>
        /// Set the bet for the player.
        /// </summary>
        /// <param name="bet">The bet.</param>
        public void SetBet(int bet) {
            Contract.Requires(this.Game != null && bet >= 0);
            this.Bet = bet;
            this.Game.Round.PlayerBet(this, bet);
            if(BetSet != null) BetSet(bet);
        }

        /// <summary>
        /// Set the game for the player.
        /// </summary>
        /// <param name="game">The game.</param>
        public void SetGame(Game game) {
            Contract.Requires(game != null);
            this.Game = game;
        }

        /// <summary>
        /// Get a playable card.
        /// </summary>
        /// <returns>The first playable card.</returns>
        [Pure]
        public Card GetPlayableCard() {
            Contract.Requires(Game != null && Game.Active && !Game.Round.BoardCards.HasPlayed(this) && Hand.Count > 0);
            var toPlay = Hand[0];
            if(Game.Round.BoardCards.FirstCard != null) {
                foreach (var card in this.Hand.Where(card => this.CardPlayable(card, this.Game.Round.BoardCards.FirstCard))) {
                    toPlay = card;
                    break;
                }
            };
            return toPlay;
        }

        /// <summary>
        /// Get a textual representation of the player.
        /// </summary>
        /// <returns>A textual representation of the player.</returns>
        [Pure]
        public override string ToString() {
            return Name;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(CardsOnHand >= 0 && CardsOnHand <= 10);
            Contract.Invariant(Tricks >= 0 && Tricks <= 10);
        }
    }
}
