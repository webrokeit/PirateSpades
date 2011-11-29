﻿using System.Diagnostics.Contracts;
using System;

namespace PirateSpades.GameLogic {
    using System.Text.RegularExpressions;

    public class Card : IComparable {

        public Card(Suit s, CardValue v) {
            this.Suit = s;
            this.Value = v;
        }

        public CardValue Value { get; private set; }

        public Suit Suit { get; private set; }

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
            Contract.Ensures(this.Suit != c.Suit || Contract.Result<bool>());
            return Suit == c.Suit;
        }

        public override string ToString() {
            return "card:{" + Suit.ToString() + "," + Value.ToString() + "}";
        }

        public static Card FromString(string s) {
            Contract.Requires(Regex.IsMatch(s, @"card:\{([A-Z]{4}),(\w{3,5})\}"));
            var m = Regex.Match(s, @"card:\{[A-Z]{4},\w{3,5}\}");
            if(m.Success) {
                Suit suit;
                if (Enum.TryParse(m.Groups[1].Value, true, out suit)) {
                    CardValue value;
                    if (Enum.TryParse(m.Groups[2].Value, true, out value)) {
                        return new Card(suit, value);
                    }
                }
            }
            return null;
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
