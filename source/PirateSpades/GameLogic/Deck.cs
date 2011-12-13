// <copyright file="Deck.cs">
//      ahal@itu.dk,
//      mche@itu.dk
// </copyright>
// <summary>
//      A representation of a deck of cards in the Pirate Spades Game.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>
// <author>Morthen Chabert Eskesen (mche@itu.dk)</author>

namespace PirateSpades.GameLogic {
    using System.Collections.Generic;
    using System.Collections;
    using System.Diagnostics.Contracts;

    using PirateSpades.Misc;

    /// <summary>
    /// A representation of a deck of cards in the Pirate Spades Game.
    /// </summary>
    public class Deck : IEnumerable<Card> {
        /// <summary>
        /// Lazy initialized list of cards in a deck of cards (52 cards).
        /// </summary>
        private static List<Card> mainDeck;

        /// <summary>
        /// The list of cards in the deck.
        /// </summary>
        private List<Card> TheDeck { get; set; }

        /// <summary>
        /// Private constructor called by the factory method GetShuffledDeck.
        /// </summary>
        /// <param name="deck">The list of cards in the deck.</param>
        private Deck(List<Card> deck) {
            this.TheDeck = deck;
        }

        /// <summary>
        /// Create the mainDeck.
        /// </summary>
        private static void CreateDeck() {
            mainDeck = new List<Card>();
            var suits = new List<Suit>() { Suit.Clubs, Suit.Diamonds, Suit.Hearts, Suit.Spades };
            var values = new List<CardValue>() {
                CardValue.Two,
                CardValue.Three,
                CardValue.Four,
                CardValue.Five,
                CardValue.Six,
                CardValue.Seven,
                CardValue.Eight,
                CardValue.Nine,
                CardValue.Ten,
                CardValue.Jack,
                CardValue.Queen,
                CardValue.King,
                CardValue.Ace
            };
            foreach(var s in suits) {
                foreach(var v in values) {
                    mainDeck.Add(new Card(s, v));
                }
            }
        }

        /// <summary>
        /// Remove the top most card and return it.
        /// </summary>
        /// <returns>The top most card.</returns>
        public Card Pop() {
            Contract.Requires(TheDeck != null && TheDeck.Count > 0);
            var c = TheDeck[TheDeck.Count - 1];
            TheDeck.RemoveAt(TheDeck.Count - 1);
            return c;
        }

        /// <summary>
        /// Get a shuffled deck.
        /// </summary>
        /// <returns>A shuffled deck.</returns>
        public static Deck GetShuffledDeck() {
            if(mainDeck == null) CreateDeck();
            return ShuffleDeck();
        }

        /// <summary>
        /// Clone the main deck, shuffle it and return the shuffled deck.
        /// </summary>
        /// <returns>A shuffled deck.</returns>
        private static Deck ShuffleDeck() {
            Contract.Requires(mainDeck != null);
            var deckClone = new List<Card>(mainDeck);
            CollectionFnc.FisherYatesShuffle(deckClone);
            return new Deck(deckClone);
        }

        /// <summary>
        /// Get an enumerator of the cards in the deck.
        /// </summary>
        /// <returns>An enumerator of the cards in the deck.</returns>
        public IEnumerator<Card> GetEnumerator() {
            return TheDeck.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator of the cards in the deck.
        /// </summary>
        /// <returns>An enumerator of the cards in the deck.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
