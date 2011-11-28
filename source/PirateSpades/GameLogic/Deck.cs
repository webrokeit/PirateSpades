using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace PirateSpades.GameLogic {
    
    public class Deck {
        private static Stack<Card> deck;

        private static void CreateDeck() {
            deck = new Stack<Card>();
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

        public static Stack<Card> ShuffleDeck() {
            if(deck == null) {
                CreateDeck();
            }
            var deckClone = new Stack<Card>(deck);
            Func.FisherYatesAlg.Algorithm(deckClone);
            return deckClone;
        }
    }
}
