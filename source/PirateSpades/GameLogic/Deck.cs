// <copyright file="Deck.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A representation of a deck of cards in the Pirate Spades Game.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.GameLogic {
    using System.Collections.Generic;
    using System.Collections;
    using System.Diagnostics.Contracts;

    using PirateSpades.Misc;

    public class Deck : IEnumerable<Card> {
        private static List<Card> mainDeck;
        private List<Card> TheDeck { get; set; }

        private Deck(List<Card> deck) {
            this.TheDeck = deck;
        }

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

        public Card Pop() {
            Contract.Requires(TheDeck != null && TheDeck.Count > 0);
            var c = TheDeck[TheDeck.Count - 1];
            TheDeck.RemoveAt(TheDeck.Count - 1);
            return c;
        }

        public static Deck GetShuffledDeck() {
            if(mainDeck == null) CreateDeck();
            return ShuffleDeck();
        }

        private static Deck ShuffleDeck() {
            Contract.Requires(mainDeck != null);
            var deckClone = new List<Card>(mainDeck);
            CollectionFnc.FisherYatesShuffle(deckClone);
            return new Deck(deckClone);
        }

        public IEnumerator<Card> GetEnumerator() {
            return TheDeck.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
