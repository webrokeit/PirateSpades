// <copyright file="Round.cs">
//      mche@itu.dk
// </copyright>
// <summary>
//      A round for the PirateSpades game.
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>

namespace PirateSpades.GameLogic {
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

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
            dealer.IsDealer = true;
            cards = players.Count * deal;
            this.number = number;
            bets = new Dictionary<Player, int>();
        }

        public int PlayerCards { get { return deal; } }

        public int TotalCards { get { return cards; } }

        public int Players { get { return players.Count; } }

        public int Bets { get { return bets.Count; } }

        public int Number { get { return number; } }

        public int NumberOfCardsPlayed { get { return table.CardsPlayed; } }

        [Pure]
        public bool IsFinished() {
            Contract.Ensures(NumberOfCardsPlayed == TotalCards ? Contract.Result<bool>() : true);
            return NumberOfCardsPlayed == TotalCards;
        }

        [Pure]
        public Player DealingPlayer() {
            Contract.Ensures(Contract.Result<Player>().IsDealer == true);
            return dealer;
        }

        public void CollectBet(Player p, int bet) {
            Contract.Requires(p != null && bet >= 0);
            bets.Add(p, bet);
            p.Bet = bet;
        }

        [Pure]
        public bool HasPlayerBet(Player p) {
            Contract.Requires(p != null);
            return bets.ContainsKey(p);
        }

        [Pure]
        public int PlayerBet(Player p) {
            Contract.Requires(p != null && HasPlayerBet(p));
            Contract.Ensures(Contract.Result<int>() >= 0);
            return bets[p];
        }

        [Pure]
        public int NumberOfTricks(Player p) {
            Contract.Requires(p != null);
            Contract.Ensures(Contract.Result<int>() >= 0);
            return p.Tricks;
        }

        [Pure]
        public bool MatchTrick(Player p) {
            Contract.Requires(p != null);
            Contract.Ensures(this.NumberOfTricks(p) == this.PlayerBet(p) ? Contract.Result<bool>() : true);
            return this.NumberOfTricks(p) == this.PlayerBet(p);
        }

        public void RemovePlayer(Player p) {
            players.Remove(p);
        }

        public void Start() {
            table.CardsPlayed = 0;
            table.SetPlayers(players);
            table.StartingPlayer = players[0];
            table.PlayerTurn = players[0];
            dealer.DealCards(players, deal);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(Players >= 0 && Players <= 5);
            Contract.Invariant(TotalCards == (Players * PlayerCards));
            Contract.Invariant(NumberOfCardsPlayed >= 0);
        }
    }
}
