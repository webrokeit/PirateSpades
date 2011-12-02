// <copyright file="Player.cs">
//      mche@itu.dk
// </copyright>
// <summary>
//      A player for the PirateSpades game.
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>

namespace PirateSpades.GameLogic {
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public class Player {
        private readonly List<Card> hand;
        private int bet;
        private readonly List<List<Card>> tricks;
        private readonly Table table = Table.GetTable();

        public Player(string name) {
            this.Name = name;
            hand = new List<Card>();
            this.IsDealer = false;
            tricks = new List<List<Card>>();
        }

        public string Name { get; protected set; }

        protected delegate void CardPlayedDelegate(Card c);

        protected delegate void CardDealtDelegate(Player p, Card c);

        protected delegate void BetSetDelegate(int bet);

        protected event CardPlayedDelegate CardPlayed;
        protected event CardDealtDelegate CardDealt;
        protected event BetSetDelegate BetSet;

        public bool IsDealer { get; set; }

        public Card CardToPlay { get; set; }

        public int NumberOfCards {
            get {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return hand.Count;
            }
        }

        public int Tricks {
            get {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return tricks.Count;
            }
        }

        public List<Card> GetHand() {
            return hand;
        }

        public Card GetPlayableCard() {
            int i = 0;
            while(!this.Playable(this.Hand(i))) {
                i++;
            }
            return this.Hand(i);
        }

        public Card Hand(int idx) {
            Contract.Requires(idx >= 0 && idx < NumberOfCards);
            Contract.Ensures(this.HaveCard(Contract.Result<Card>()));
            return hand[idx];
        }

        public int Bet {
            get {
                Contract.Ensures(Contract.Result<int>() >= 0);
            return bet;
            } set {
                Contract.Requires(value >= 0);
                bet = value;
                if(BetSet != null) {
                    BetSet(value);
                }
            }
        }

        public void ReceiveTrick(List<Card> trick) {
            Contract.Requires(trick != null);
            Contract.Ensures(Tricks == Contract.OldValue(Tricks) + 1);
            tricks.Add(new List<Card>(trick));
        }

        public void ClearTricks() {
            Contract.Ensures(Tricks == 0 && Bet == 0);
            tricks.Clear();
            Bet = 0;
        }

        public bool HaveCard(Card c) {
            Contract.Requires(c != null);
            return hand.Contains(c);
        }

        public bool AnyCard(Suit s) {
            return hand.Any(c => c.Suit == s);
        }

        public bool Playable(Card c) {
            Contract.Requires(c != null);
            Contract.Ensures(Contract.OldValue(table.OpeningCard) == null || c.Suit == table.OpeningCard.Suit || !this.AnyCard(table.OpeningCard.Suit) ? Contract.Result<bool>() : true);
            if(table.OpeningCard == null) {
                return true;
            }
            if(c.Suit == table.OpeningCard.Suit) {
                return true;
            }
            return !this.AnyCard(table.OpeningCard.Suit);
        }

        public void PlayCard(Card c) {
            Contract.Requires(c != null);
            Contract.Requires(this.HaveCard(c));
            Contract.Requires(this.Playable(c));
            Contract.Ensures(!this.HaveCard(c) && NumberOfCards == Contract.OldValue(NumberOfCards) - 1);
            CardToPlay = c;
            if(CardPlayed != null) {
                CardPlayed(c);
            }
            table.ReceiveCard(this, c);
            hand.Remove(c);
        }

        public void ReceiveCard(Card c) {
            Contract.Requires(c != null);
            Contract.Ensures(this.HaveCard(c) && NumberOfCards == Contract.OldValue(NumberOfCards) + 1);
            hand.Add(c);
        }

        public void DealCards(List<Player> players, int deal) {
            Contract.Requires(players != null);
            Contract.Requires(deal > 0);
            Contract.Requires(IsDealer);
            Contract.Ensures(NumberOfCards == deal);
            Deck deck = Deck.ShuffleDeck();
            for(int i = 0; i < deal; i++) {
                foreach(var p in players) {
                    var c = deck.Pop();
                    if(CardDealt != null) {
                        CardDealt(p, c);
                    }
                    p.ReceiveCard(c);
                }
            }
        }

        public override string ToString() {
            return Name;
        }
        
        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(NumberOfCards >= 0 && NumberOfCards <= 10);
            Contract.Invariant(Tricks >= 0);
        }
    }
}
