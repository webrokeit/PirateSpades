using System.Collections.Generic;

namespace PirateSpades.GameLogic {
    using System.Collections;
    using System.Diagnostics.Contracts;

    public class Deck : IEnumerable<Card> {
        private static List<Card> mainDeck;
        private List<Card> deck { get; set; }

        private Deck(List<Card> deck) {
            this.deck = deck;
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
            Contract.Requires(deck != null && deck.Count > 0);
            var c = deck[deck.Count - 1];
            deck.RemoveAt(deck.Count - 1);
            return c;
        }

        public static Deck ShuffleDeck() {
            if(mainDeck == null) {
                CreateDeck();
            }
            var deckClone = new List<Card>(mainDeck);
            Func.FisherYatesAlg.Algorithm(deckClone);
            return new Deck(deckClone);
        }

        public IEnumerator<Card> GetEnumerator() {
            return ((IEnumerable<Card>)this.deck).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
