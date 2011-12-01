namespace PirateSpades.GameLogicV2 {
    using System.Diagnostics.Contracts;
    using System;
    using System.Text.RegularExpressions;

    public class Card : IComparable {

        public Card(Suit s, CardValue v) {
            this.Suit = s;
            this.Value = v;
        }

        public CardValue Value { get; private set; }

        public Suit Suit { get; private set; }

        public int CompareTo(Object obj) {
            if(obj == null || !(obj is Card)) return 0;
            return this.HigherThan((Card)obj) ? -1 : 1;
        }

        public bool HigherThan(Card card) {
            Contract.Requires(card != null);
            if(Suit == Suit.Spades && card.Suit != Suit) return true;
            if(Suit == card.Suit && Value > card.Value) return true;
            return false;
        }

        public bool SameSuit(Card card) {
            Contract.Requires(card != null);
            Contract.Ensures(this.Suit != card.Suit || Contract.Result<bool>());
            return Suit == card.Suit;
        }

        public override string ToString() {
            return "card: " + Suit.ToString() + ";" + Value.ToString();
        }

        public static int CardsToDeal(int round, int players) {
            var maxCards = 52 / players < 10 ? 52 / players : 10;
            return (round <= maxCards ? maxCards - round + 1 : round - maxCards);
        }

        public static Card FromString(string s) {
            Contract.Requires(Regex.IsMatch(s, @"^card: \w{5,8};\w{3,5}$", RegexOptions.Multiline));
            var m = Regex.Match(s, @"^card: (\w{5,8});(\w{3,5})$", RegexOptions.Multiline);
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
