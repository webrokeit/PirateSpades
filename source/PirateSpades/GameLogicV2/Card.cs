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

        [Pure]
        public int CompareTo(Object obj) {
            if(obj == null || !(obj is Card)) return 0;
            var h = this.GetHashCode();
            var oh = obj.GetHashCode();

            if(h > oh) return 1;
            if(h < oh) return -1;
            return 0;
        }

        [Pure]
        public bool HigherThan(Card card) {
            Contract.Requires(card != null);
            if(Suit == Suit.Spades && card.Suit != Suit) return true;
            if(Suit == card.Suit && Value > card.Value) return true;
            return false;
        }

        [Pure]
        public bool SameSuit(Card card) {
            Contract.Requires(card != null);
            Contract.Ensures(this.Suit != card.Suit || Contract.Result<bool>());
            return Suit == card.Suit;
        }

        [Pure]
        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);
            return "card: " + Suit.ToString() + ";" + Value.ToString();
        }

        [Pure]
        public static Card FromString(string s) {
            Contract.Requires(Regex.IsMatch(s, "^card: " + EnumRegexString(typeof(Suit)) + ";" + EnumRegexString(typeof(CardValue)) + "$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<Card>() != null);
            var m = Regex.Match(s, "^card: " + EnumRegexString(typeof(Suit)) + ";" + EnumRegexString(typeof(CardValue)) + "$", RegexOptions.Multiline);
            var suit = (Suit) Enum.Parse(typeof(Suit), m.Groups[1].Value, true);
            var value = (CardValue) Enum.Parse(typeof(CardValue), m.Groups[2].Value, true);
            return new Card(suit, value);
        }

        [Pure]
        public static int CardsToDeal(int round, int players) {
            var maxCards = 52 / players < 10 ? 52 / players : 10;
            return (round <= maxCards ? maxCards - round + 1 : round - maxCards);
        }

        [Pure]
        public override int GetHashCode() {
            var h = (int)Math.Pow(10, (int)Suit) + (int)Value;
            return h;
        }

        [Pure]
        public override bool Equals(object obj) {
            if (!(obj is Card)) {
                return false;
            }
            if (obj.GetHashCode() != this.GetHashCode()) {
                return false;
            }
            if (((Card)obj).Suit != Suit) {
                return false;
            }
            return ((Card)obj).Value == this.Value;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(Value > (CardValue)1 && Value < (CardValue)15);
        }

        [Pure]
        public static string EnumRegexString(Type enumType) {
            Contract.Requires(enumType != null && Enum.GetNames(enumType).Length > 0);
            var names = Enum.GetNames(enumType);
            var res = new string[names.Length];
            for(var i = 0; i < names.Length; i++) {
                res[i] = Regex.Escape(names[i]);
            }
            return "(" + string.Join("|", res) + ")";
        }
    }

    public enum Suit {
        Diamonds = 1,
        Clubs = 2,
        Hearts = 3,
        Spades = 4
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
