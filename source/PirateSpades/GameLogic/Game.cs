using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace PirateSpades.GameLogic {
    using System;
    using System.Linq;

    public class Game {
        private List<Player> players;
        private List<Player> dealership;
        private int rounds;
        private int playedRounds;
        private bool started;
        private Player dealer;
        private Table table = Table.GetTable();
        private Dictionary<Player, int> points;
        private Dictionary<Round, Dictionary<Player, int>> roundPoints;

        public Game() {
            players = new List<Player>();
            dealership = new List<Player>();
            rounds = 20;
            playedRounds = 0;
            started = false;
            points = new Dictionary<Player, int>();
            roundPoints = new Dictionary<Round, Dictionary<Player, int>>();
            CurrentRound = null;
        }

        public int Players { get { return players.Count; } }

        public int Rounds { get { return rounds; } }

        public Round CurrentRound { get; private set; }

        public int RoundsPlayed { get { return playedRounds; } }

        public bool IsStarted { get { return started; } }

        public int RoundsLeft() {
            return Rounds - RoundsPlayed;
        }

        public Player PlayerToDeal() {
            Contract.Ensures(Contract.Result<Player>().IsDealer == true);
            return dealer;
        }

        public bool IsFinished() {
            Contract.Ensures(this.RoundsLeft() == 0 ? Contract.Result<bool>() : true);
            return this.RoundsLeft() == 0;
        }

        public void AddPlayer(Player p) {
            Contract.Requires(p != null && Players < 5 && !IsStarted);
            Contract.Ensures(Players == Contract.OldValue(Players) + 1);
            players.Add(p);
            points.Add(p, 0);
            if(dealership.Count == 0) {
                dealer = p;
            }
            dealership.Add(p);
            table.AddPlayer(p);
        }

        public void RemovePlayer(Player p) {
            players.Remove(p);
            dealership.Remove(p);
            table.RemovePlayer(p);
            CurrentRound.RemovePlayer(p);
        }

        public void ClearPlayers() {
            players.Clear();
        }

        public List<int> PointsFromRound(int number) {
            Contract.Requires(number > 0 && number <= 20);
            Round r = this.roundPoints.Keys.FirstOrDefault(key => key.Number == number);
            var roundp = this.players.Select(p => this.roundPoints[r][p]).ToList();
            return roundp;
        } 

        public int Points(Player p) {
            Contract.Requires(p != null);
            return this.points[p];
        }

        public void GivePoints(Player p, int point) {
            Contract.Requires(p != null);
            Contract.Ensures(points[p] == Contract.OldValue(points[p]) + point);
            points[p] += point;
        }

        public bool HasAllPlayersBet(Round r) {
            return this.players.All(r.HasPlayerBet);
        }

        public void StartTable(Round r) {
            Contract.Requires(this.HasAllPlayersBet(r));
            table.Start(this);
        }

        public void ReceiveStats(Round r) {
            Contract.Requires(r != null && r.IsFinished());
            roundPoints.Add(r, new Dictionary<Player, int>());
            foreach(var p in players) {
                if(r.MatchTrick(p)) {
                    int pluspoints = 10 + r.NumberOfTricks(p);
                    this.GivePoints(p, pluspoints);
                    roundPoints[r].Add(p, pluspoints);
                    p.ClearTricks();
                    continue;
                } 
                if(r.NumberOfTricks(p) < r.PlayerBet(p)) {
                    int minuspoints = r.NumberOfTricks(p) - r.PlayerBet(p);
                    this.GivePoints(p, minuspoints);
                    roundPoints[r].Add(p, minuspoints);
                    p.ClearTricks();
                } else {
                    int minuspoints = r.PlayerBet(p) - r.NumberOfTricks(p);
                    this.GivePoints(p, minuspoints);
                    roundPoints[r].Add(p, minuspoints);
                    p.ClearTricks();
                }
            }
        }

        public void Start() {
            Contract.Requires(Players >= 2 && Players <= 5 && !IsStarted);
            Contract.Ensures(IsStarted);
            started = true;
            int roundNumber = 1;
            int deal = 10;
            while(!IsFinished()) {
                this.RotateDealer();
                var r = new Round(dealership, deal, roundNumber);
                CurrentRound = r;
                // Collect bet from each player
                r.Start();
                this.ReceiveStats(r);
                roundNumber++;
                playedRounds++;
                if(playedRounds < 11) {
                    deal--;
                } else if(playedRounds == 11) {
                    //DO NOTHING
                } else {
                    deal++;
                }
            }
        }

        public void RotateDealer() {
            Contract.Ensures(Contract.OldValue(dealer).IsDealer == false);
            Contract.Ensures(dealer.IsDealer == true);
            if(dealer != null) {
                dealer.IsDealer = false;
            }
            Player p = dealership[0];
            this.dealer = p;
            dealer.IsDealer = true;
            dealership.Remove(p);
            dealership.Add(p);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(RoundsPlayed <= Rounds);
            Contract.Invariant(this.RoundsLeft() >= 0);
        }
    }
}
