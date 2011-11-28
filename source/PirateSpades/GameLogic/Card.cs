using System.Diagnostics.Contracts;
using System;

namespace PirateSpades.GameLogic {
    
    public class Card : IComparable {
        private readonly Suit s;
        private readonly CardValue v;

        public Card(Suit s, CardValue v) {
            this.s = s;
            this.v = v;
        }

        public CardValue Value { get { return v; } }

        public Suit Suit { get { return s; } }

        public int CompareTo(Object obj) {
            Contract.Requires(obj != null && obj is Card);
            var c = (Card)obj;
            if(c.Suit == Suit) {
                if(Value < c.Value) {
                    return -1;
                }
                return 1;
            }
            if(Suit == Suit.Spades) {
                return 1;
            }
            return -1;
        }

        public bool SameSuit(Card c) {
            Contract.Requires(c != null);
            Contract.Ensures(Suit == c.Suit ? Contract.Result<bool>() : true);
            return Suit == c.Suit;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(Value > (CardValue)1 && Value < (CardValue)15);
        }
    }

    public enum Suit {
        Hearts, Spades, Diamonds, Clubs
    }

    public enum CardValue {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }
}
