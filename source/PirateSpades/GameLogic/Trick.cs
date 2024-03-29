﻿// <copyright file="Trick.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A representation of a trick in the Pirate Spades Game.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.GameLogic {
    using System.Diagnostics.Contracts;

    using PirateSpades.Misc;

    public class Trick {
        public ImmutableOrderedDictionary<Player, Card> Pile { get; private set; }
        private OrderedDictionary<Player, Card> CardsPlayed { get; set; }
        public Card FirstCard { get; private set; }

        public Player Winner {
            get {
                Contract.Requires(CardsPlayed.Count > 0 && FirstCard != null);
                Player winner = null;
                Card card = null;
                foreach(var key in CardsPlayed.Keys) {
                    if (winner == null) {
                        winner = key;
                        card = CardsPlayed[key];
                    } else if (CardsPlayed[key].SameSuit(FirstCard) || CardsPlayed[key].Suit == Suit.Spades) {
                        if (CardsPlayed[key].HigherThan(card)) {
                            winner = key;
                            card = CardsPlayed[key];
                        }
                    }
                }
                return winner;
            }
        }

        public Trick() {
            CardsPlayed = new OrderedDictionary<Player, Card>();
            this.UpdatePile();
        }

        private void UpdatePile() {
            Pile = CardsPlayed.AsImmutable();
        }

        public void PlaceCard(Player player, Card card) {
            Contract.Requires(player != null && card != null && !Pile.ContainsKey(player));
            if(FirstCard == null) FirstCard = card;
            CardsPlayed.Add(player, card);
            this.UpdatePile();
        }

        [Pure]
        public bool HasPlayed(Player player) {
            Contract.Requires(player != null);
            return Pile.ContainsKey(player);
        }
    }
}
