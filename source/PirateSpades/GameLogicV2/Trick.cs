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
        public Card FirstCard { get; private set; }

        public Player Winner {
            get {
                Contract.Requires(CardsPlayed.Count > 0 && FirstCard != null);
                Player winner = null;
                foreach(KeyValuePair<Player, Card> kvp in CardsPlayed) {
                    if (winner == null) {
                        winner = kvp.Key;
                    } else if (kvp.Value.SameSuit(FirstCard) || kvp.Value.Suit == Suit.Spades) {
                        if (kvp.Value.HigherThan(FirstCard)) {
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
            if(FirstCard == null) FirstCard = card;
            Pile.Add(player, card);
            this.UpdatePile();
        }

        public bool HasPlayed(Player player) {
            Contract.Requires(player != null);
            return Pile.ContainsKey(player);
        }
    }
}
