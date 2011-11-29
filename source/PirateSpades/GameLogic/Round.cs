using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PirateSpades.GameLogic {
    public class Round {
        private List<Player> players;
        private int deal;
        private Table table = Table.GetTable();
        private int cards;
        private Dictionary<Player, int> bets;
        private Player dealer;
        private int number;

        public Round(List<Player> players, int deal, int number) {
            Contract.Ensures(deal == PlayerCards);
            this.players = players;
            this.deal = deal;
            dealer = players.Last();
            cards = players.Count * deal;
            this.number = number;
        }

        public int PlayerCards { get { return deal; } }

        public int TotalCards { get { return cards; } }

        public int Players { get { return players.Count; } }

        public int Bets { get { return bets.Count; } }

        public int NumberOfCardsPlayed { get { return table.CardsPlayed; } }

        public bool IsFinished() {
            Contract.Ensures(NumberOfCardsPlayed == TotalCards ? Contract.Result<bool>() : true);
            return NumberOfCardsPlayed == TotalCards;
        }

        public Player DealingPlayer() {
            Contract.Ensures(Contract.Result<Player>().IsDealer == true);
            return dealer;
        }

        public void CollectBet(Player p, int bet) {
            Contract.Requires(p != null && bet >= 0);
            bets.Add(p, bet);
            p.Bet = bet;
        }

        public int PlayerBet(Player p) {
            Contract.Requires(p != null && bets.ContainsKey(p));
            Contract.Ensures(Contract.Result<int>() >= 0);
            return bets[p];
        }

        public int NumberOfTricks(Player p) {
            Contract.Requires(p != null);
            Contract.Ensures(Contract.Result<int>() >= 0);
            return p.Tricks;
        }

        public bool MatchTrick(Player p) {
            Contract.Requires(p != null);
            Contract.Ensures(this.NumberOfTricks(p) == this.PlayerBet(p) ? Contract.Result<bool>() : true);
            return this.NumberOfTricks(p) == this.PlayerBet(p);
        }

        public void Start() {
            //Contract.Requires(Bets == Players);
            table.AddPlayers(players);
            table.StartingPlayer = players[0];
            table.PlayerTurn = players[0];
            dealer.DealCards(players, deal);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(Players >= 0 && Players <= 5);
            Contract.Invariant(PlayerCards >= Players);
            Contract.Invariant(TotalCards > 0 && NumberOfCardsPlayed > 0);
        }
    }
}
