using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PirateSpades.GameLogic {
    public class Player {
        private readonly List<Card> hand;
        private int bet;
        private readonly List<List<Card>> tricks;
        private Card playCard;
        private Deck deck = Deck.GetDeck();

        public Player() {
            hand = new List<Card>();
            this.IsDealer = false;
            tricks = new List<List<Card>>();
        }

        public bool IsDealer { get; set; }

        public Card CardToPlay {
            get {
                return playCard;
            }
            set { playCard = value; }
        }

        public int NumberOfCards {
            get {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return hand.Count;
            }
        }

        public int Tricks {
            get {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return tricks.Count;
            }
        }

        public Card Hand(int idx) {
            Contract.Requires(idx <= 0 && idx < NumberOfCards);
            return hand[idx];
        }

        public int Bet {
            get {
                Contract.Ensures(Contract.Result<int>() >= 0);
            return bet;
            } set {
                Contract.Requires(value >= 0);
                bet = value;
            }
        }

        public void ReceiveTrick(List<Card> trick) {
            Contract.Requires(trick != null);
            Contract.Ensures(Tricks == Contract.OldValue(Tricks) + 1);
            tricks.Add(trick);
        }

        public void ClearTricks() {
            Contract.Ensures(Tricks == 0);
            foreach(var t in tricks) {
                foreach(var card in t) {
                    deck.AddCard(card);
                }
            }
            tricks.Clear();
        }

        public bool HaveCard(Card c) {
            Contract.Requires(c != null);
            return hand.Contains(c);
        }

        public bool AnyCard(Suit s) {
            return hand.Any(c => c.Suit == s);
        }

        public bool Playable(Card c) {
            Contract.Requires(c != null);
            Contract.Ensures(c.Suit == Table.OpeningCard.Suit ? Contract.Result<bool>() : true ||
                !this.AnyCard(Table.OpeningCard.Suit) ? Contract.Result<bool>() : true);
            if(c.Suit == Table.OpeningCard.Suit) {
                return true;
            }
            return !this.AnyCard(Table.OpeningCard.Suit);
        }

        public void PlayCard(Card c) {
            Contract.Requires(c != null && this.Playable(c) && this.HaveCard(c));
            Contract.Ensures(!this.HaveCard(c) && NumberOfCards == Contract.OldValue(NumberOfCards) - 1);
            CardToPlay = c;
            Table.ReceiveCard(this);
            hand.Remove(c);
        }

        public void ReceiveCard(Card c) {
            Contract.Requires(c != null);
            Contract.Ensures(this.HaveCard(c) && NumberOfCards == Contract.OldValue(NumberOfCards) + 1);
            hand.Add(c);
        }

        public void DealCards(List<Player> players, int deal) {
            Contract.Requires(players != null && deck.Count == 52 && deal > 0 && IsDealer);
            Contract.Ensures(NumberOfCards == deal);
            deck.Shuffle();
            for(int i = 0; i < deal; i++) {
                foreach(var p in players) {
                    p.ReceiveCard(deck.RemoveTopCard());
                }
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(NumberOfCards >= 0 && NumberOfCards <= 10);
            Contract.Invariant(Tricks >= 0);
        }
    }
}
