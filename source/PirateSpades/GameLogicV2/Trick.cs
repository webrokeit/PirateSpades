using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.GameLogicV2 {
    using System.Diagnostics.Contracts;

    using PirateSpades.Misc;

    public class Trick {
        public ImmutableOrderedDictionary<Player, Card> Pile { get; private set; }
        private OrderedDictionary<Player, Card> CardsPlayed { get; set; }

        public Player Winner {
            get {
                Contract.Requires(CardsPlayed.Count > 0);
                Player winner = null;
                Card firstCard = null;
                foreach(KeyValuePair<Player, Card> kvp in CardsPlayed) {
                    if (firstCard == null) {
                        firstCard = kvp.Value;
                    }

                    if (winner == null) {
                        winner = kvp.Key;
                    } else if (kvp.Value.SameSuit(firstCard) || kvp.Value.Suit == Suit.Spades) {
                        if (kvp.Value.HigherThan(firstCard)) {
                            winner = kvp.Key;
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
            Pile.Add(player, card);
            this.UpdatePile();
        }
    }
}
