namespace PirateSpades.GameLogic {
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public class Player {
        public Game Game { get; private set; }
        public string Name { get; protected set; }
        public int Bet { get; private set; }
        public bool IsDealer { get; set; }
        public Card CardToPlay { get; private set; }

        public IList<Card> Hand { get; protected set; }
        protected SortedSet<Card> Cards { get; set; }

        public int CardsOnHand {
            get {
                return Hand.Count;
            }
        }

        public int Tricks {
            get {
                return Game != null && Game.Started && Game.Round.PlayerTricks.ContainsKey(this)
                           ? Game.Round.PlayerTricks[this].Count
                           : 0;
            }
        }

        protected delegate void CardPlayedDelegate(Card c);
        protected delegate void CardDealtDelegate(Player p, Card c);
        protected delegate void BetSetDelegate(int bet);
        protected event CardPlayedDelegate CardPlayed;
        protected event CardDealtDelegate CardDealt;
        protected event BetSetDelegate BetSet;

        public Player(string name) {
            this.Name = name;
            this.Cards = new SortedSet<Card>();
            this.Hand = Cards.ToList().AsReadOnly();
            this.IsDealer = false;
            this.UpdateHand();
        }

        private void UpdateHand() {
            lock (this.Cards) {
                this.Hand = this.Cards.ToList().AsReadOnly();
            }
        }

        public void GetCard(Card card) {
            Contract.Requires(card != null);
            lock (this.Cards) {
                this.Cards.Add(card);
            }
            this.UpdateHand();
        }

        public void RemoveCard(Card card) {
            Contract.Requires(card != null && this.Cards.Contains(card));
            lock (this.Cards) {
                this.Cards.Remove(card);
            }
            this.UpdateHand();
        }

        public void ClearHand() {
            this.Cards.Clear();
            this.UpdateHand();
        }

        public void PlayCard(Card card) {
            Contract.Requires(card != null && this.HasCard(card) && this.CardPlayable(card, Game.Round.BoardCards.FirstCard));
            Contract.Ensures(!this.HasCard(card) && CardsOnHand == Contract.OldValue(CardsOnHand) - 1);
            CardToPlay = card;
            Game.Round.PlayCard(this, card);
            this.RemoveCard(card);
            if(CardPlayed != null) CardPlayed(card);
        }

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

        [Pure]
        public bool CardPlayable(Card toPlay) {
            Contract.Requires(toPlay != null);
            Contract.Requires(this.HasCard(toPlay));
            Contract.Requires(this.Game.Started && this.Game.Round != null);
            return CardPlayable(toPlay, Game.Round.BoardCards.FirstCard);
        }

        [Pure]
        public bool CardPlayable(Card toPlay, Card mustMatch) {
            Contract.Requires(toPlay != null);
            Contract.Requires(this.HasCard(toPlay));
            return mustMatch == null || (!this.HasCardOf(mustMatch.Suit) || toPlay.SameSuit(mustMatch));
        }

        [Pure]
        public bool HasCardOf(Suit suit) {
            return Hand.Any(c => c.Suit == suit);
        }

        [Pure]
        public bool HasCard(Card card) {
            lock (Cards) {
                return this.Cards.Contains(card);
            }
        }

        public void SetBet(int bet) {
            Contract.Requires(this.Game != null && bet >= 0);
            this.Bet = bet;
            this.Game.Round.PlayerBet(this, bet);
            if(BetSet != null) BetSet(bet);
        }

        public void SetGame(Game game) {
            Contract.Requires(game != null);
            this.Game = game;
        }

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

        [Pure]
        public override string ToString() {
            return Name;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(CardsOnHand >= 0 && CardsOnHand <= 10, "[" + Name + "] Cards: " + CardsOnHand);
            Contract.Invariant(Tricks >= 0, "[" + Name + "] Tricks: " + Tricks);
        }
    }
}
