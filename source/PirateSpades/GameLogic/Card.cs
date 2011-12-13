// <copyright file="Card.cs">
//      ahal@itu.dk,
//      mche@itu.dk
// </copyright>
// <summary>
//      A representation of a card in the Pirate Spades Game.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>
// <author>Morten Charbert Eskesen (mche@itu.dk)</author>

namespace PirateSpades.GameLogic {
    using System.Diagnostics.Contracts;
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A representation of a card in the Pirate Spades Game.
    /// </summary>
    public class Card : IComparable {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suit">The suit of the card.</param>
        /// <param name="value">The value of the card.</param>
        public Card(Suit suit, CardValue value) {
            Contract.Requires((int)suit >= (int)Suit.Diamonds && (int)suit <= (int)Suit.Spades);
            Contract.Requires((int)value >= (int)CardValue.Two && (int)value <= (int)CardValue.Ace);
            this.Suit = suit;
            this.Value = value;
        }

        /// <summary>
        /// The value of the card.
        /// </summary>
        public CardValue Value { get; private set; }

        /// <summary>
        /// The suit of the card.
        /// </summary>
        public Suit Suit { get; private set; }

        /// <summary>
        /// Compares an object to the current card.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// 0 if the object is not of type Card or if they are of the same value, 
        /// -1 if the current card is higher than the obj and 1 if the object is higher.
        /// </returns>
        [Pure]
        public int CompareTo(Object obj) {
            Contract.Ensures(Contract.Result<int>() >= -1 && Contract.Result<int>() <= 1);
            if(obj == null || !(obj is Card)) return 0;
            var h = this.GetHashCode();
            var oh = obj.GetHashCode();

            if(h > oh) return 1;
            if(h < oh) return -1;
            return 0;
        }

        /// <summary>
        /// Checks whether the specified card is higher than the current card.
        /// Spades are trump.
        /// </summary>
        /// <param name="card">The card to check.</param>
        /// <returns>True if the card is higher, false if not.</returns>
        [Pure]
        public bool HigherThan(Card card) {
            Contract.Requires(card != null);
            if(Suit == Suit.Spades && card.Suit != Suit) return true;
            if(Suit == card.Suit && Value > card.Value) return true;
            return false;
        }

        /// <summary>
        /// Checks whether the specified card is of the same suit as the current.
        /// </summary>
        /// <param name="card">The card to check.</param>
        /// <returns>True if they are of the same suit, false if not.</returns>
        [Pure]
        public bool SameSuit(Card card) {
            Contract.Requires(card != null);
            return Suit == card.Suit;
        }

        /// <summary>
        /// Get a textual represenation of the card.
        /// </summary>
        /// <returns>A string representing the card.</returns>
        [Pure]
        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);
            return "card: " + Suit.ToString() + ";" + Value.ToString();
        }

        /// <summary>
        /// Get a cute and small textual representation of the card.
        /// </summary>
        /// <returns>A string representing the card.</returns>
        [Pure]
        public string ToShortString() {
            var suit = "♥";
            switch(Suit) {
                case Suit.Clubs:
                    suit = "♣";
                    break;
                case Suit.Diamonds:
                    suit = "♦";
                    break;
                case Suit.Spades:
                    suit = "♠";
                    break;
            }

            var value = ((int)Value).ToString();
            switch(Value) {
                case CardValue.Ten:
                    value = "T";
                    break;
                case CardValue.Jack:
                    value = "J";
                    break;
                case CardValue.Queen:
                    value = "Q";
                    break;
                case CardValue.King:
                    value = "K";
                    break;
                case CardValue.Ace:
                    value = "A";
                    break;
            }

            return suit + value;
        }

        /// <summary>
        /// Get a card from a string.
        /// </summary>
        /// <param name="s">The string to get the card from.</param>
        /// <returns>The card.</returns>
        [Pure]
        public static Card FromString(string s) {
            Contract.Requires(s != null);
            Contract.Requires(Regex.IsMatch(s, "^card: " + EnumRegexString(typeof(Suit)) + ";" + EnumRegexString(typeof(CardValue)) + "$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<Card>() != null);
            var m = Regex.Match(s, "^card: " + EnumRegexString(typeof(Suit)) + ";" + EnumRegexString(typeof(CardValue)) + "$", RegexOptions.Multiline);
            var suit = (Suit) Enum.Parse(typeof(Suit), m.Groups[1].Value, true);
            var value = (CardValue) Enum.Parse(typeof(CardValue), m.Groups[2].Value, true);
            return new Card(suit, value);
        }

        /// <summary>
        /// Get amount of cards to deal per player.
        /// </summary>
        /// <param name="round">The round number.</param>
        /// <param name="players">Amount of players.</param>
        /// <returns>Amount of cards to deal per player.</returns>
        [Pure]
        public static int CardsToDeal(int round, int players) {
            Contract.Requires(round > 0 && players > 0);
            var maxCards = 52 / players < 10 ? 52 / players : 10;
            return (round <= maxCards ? maxCards - round + 1 : round - maxCards);
        }

        /// <summary>
        /// Get the hash code of the card.
        /// </summary>
        /// <returns>The hash code of the card.</returns>
        [Pure]
        public override int GetHashCode() {
            var h = (int)Math.Pow(10, (int)Suit) + (int)Value;
            return h;
        }

        /// <summary>
        /// Checks whether or not the specified object equals this.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if they're equal, false if not.</returns>
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
            Contract.Invariant((int)Suit >= (int)Suit.Diamonds && (int)Suit <= (int)Suit.Spades);
            Contract.Invariant((int)Value >= (int)CardValue.Two && (int)Value <= (int)CardValue.Ace);
        }

        /// <summary>
        /// Get a string for use with regex of all the specified enums possible textual values.
        /// Example: (Diamonds|Clubs|Hearts|Spades)
        /// </summary>
        /// <param name="enumType">The enum to use.</param>
        /// <returns>The regex string.</returns>
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
        /// <summary>Diamonds</summary>
        Diamonds = 1,

        /// <summary>Clubs</summary>
        Clubs = 2,

        /// <summary>Hearts</summary>
        Hearts = 3,

        /// <summary>Spades</summary>
        Spades = 4
    }

    public enum CardValue {
        /// <summary>Two</summary>
        Two = 2,

        /// <summary>Three</summary>
        Three = 3,

        /// <summary>Four</summary>
        Four = 4,

        /// <summary>Five</summary>
        Five = 5,

        /// <summary>Six</summary>
        Six = 6,

        /// <summary>Seven</summary>
        Seven = 7,

        /// <summary>Eight</summary>
        Eight = 8,

        /// <summary>Nine</summary>
        Nine = 9,

        /// <summary>Ten</summary>
        Ten = 10,

        /// <summary>Jack</summary>
        Jack = 11,

        /// <summary>Queen</summary>
        Queen = 12,

        /// <summary>King</summary>
        King = 13,

        /// <summary>Ace</summary>
        Ace = 14
    }
}
