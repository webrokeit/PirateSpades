using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace PirateSpades.GameLogic {
    
    public class Deck {
        private static readonly Deck Me = new Deck();
        private readonly Stack<Card> deck;

        private Deck() {
            deck = new Stack<Card>();
            CreateDeck();
        }

        public int Count { get { return deck.Count; } }

        public static Deck GetDeck() {
            return Me;
        }

        private void CreateDeck() {
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
                    deck.Push(new Card(s, v));
                }
            }
        }

        public void AddCard(Card c) {
            Contract.Requires(c != null && Count < 52);
            Contract.Ensures(Count == Contract.OldValue(Count) + 1);
            deck.Push(c);
        }

        public Card RemoveTopCard() {
            Contract.Requires(Count > 0);
            Contract.Ensures(Count == Contract.OldValue(Count) - 1);
            return deck.Pop();
        }

        public void Shuffle() {
            Contract.Requires(Count == 52);
            // TODO
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(Count >= 0 && Count <= 52);
        }
    }
}
