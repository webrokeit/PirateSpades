using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.GameLogicV2 {
    using System.Diagnostics.Contracts;

    using PirateSpades.Misc;

    public class Game {
        public IList<Player> Players { get; private set; }
        private OrderedDictionary<Player, int> GamePlayers { get; set; }
        private Dictionary<String, Player> PlayerNames { get; set; }
        public int CurrentDealer { get; private set; }

        public int CardsToDeal{
            get {
                return Card.CardsToDeal(CurrentRound, Players.Count);
            }
        }

        public int RoundsPossible {
            get {
                return Round.RoundsPossible(Players.Count);
            }
        }

        private Dictionary<int, Round> Rounds { get; set; }
        public int CurrentRound { get; private set; }
        public Round Round {
            get {
                Contract.Requires(Started && CurrentRound >= 1 && CurrentRound <= RoundsPossible);
                Contract.Ensures(Contract.Result<Round>() == Rounds[CurrentRound]);
                return this.GetRound(CurrentRound);
            }
        }
        public bool Started { get; private set; }
        public bool Active {
            get {
                return Started && CurrentRound >= 1 && CurrentRound <= Round.RoundsPossible(Players.Count);
            }
        }
        public bool Finished {
            get {
                return Started && !Active && CurrentRound >= 1;
            }
        }
        private bool IsHost { get; set; }

        public delegate void GameEventDelegate(Game game);

        public event GameEventDelegate RoundStarted;
        public event GameEventDelegate RoundFinished;
        public event GameEventDelegate GameFinished;

        public Game() {
            GamePlayers = new OrderedDictionary<Player, int>();
            PlayerNames = new Dictionary<string, Player>();
            this.UpdatePlayers();
            Rounds = new Dictionary<int, Round>();
            CurrentRound = 0;
            Started = false;
            IsHost = false;
            CurrentDealer = -1;
        }

        public Game(IEnumerable<Player> players) : this() {
            this.AddPlayers(players);
        }

        public void Start(int dealer) {
            Contract.Requires(dealer >= 0 && dealer < Players.Count);
            CurrentDealer = dealer;
            this.Start(false);
        }

        public void Start(bool isHost) {
            Contract.Requires(Players.Count >= 2);
            this.Started = true;
            this.IsHost = isHost;
            if(this.IsHost) {
                this.NewRound();
            }
        }

        private void Finish() {
            Contract.Requires(Finished);
            if(GameFinished != null) GameFinished(this);
        }

        public void NewRound() {
            Contract.Requires(Started);
            CurrentRound++;
            if(Active) {
                if (CurrentRound == 1) {
                    if (CurrentDealer == -1) {
                        CurrentDealer = CollectionFnc.PickRandom(GamePlayers.Values.ToList());
                    }
                } else {
                    CurrentDealer = (this.GetRound(CurrentRound - 1).Dealer + 1) % Players.Count;
                }

                var r = new Round(this, CurrentDealer);
                if(IsHost) r.RoundFinished += this.OnRoundFinished;
                Rounds.Add(CurrentRound, r);
                r.Start();
                if (RoundStarted != null) RoundStarted(this);
            } else {
                this.Finish();
            }
        }

        private void OnRoundFinished(Round round) {
            Contract.Requires(round != null && Active);
            if (RoundFinished != null) RoundFinished(this);
            if (IsHost) {
                round.RoundFinished -= this.OnRoundFinished;
                this.NewRound();
            }
        }

        public void AddPlayer(Player player) {
            Contract.Requires(player != null && !GamePlayers.ContainsKey(player) && !Started);
            GamePlayers.Add(player, GamePlayers.Count);
            this.UpdatePlayers();
        }

        public void AddPlayers(IEnumerable<Player> players) {
            Contract.Requires(players != null && !Started);
            var playersBefore = GamePlayers.Count;
            foreach (var player in players.Where(player => !this.GamePlayers.ContainsKey(player))) {
                GamePlayers.Add(player, GamePlayers.Count);
            }
            if(GamePlayers.Count > playersBefore) this.UpdatePlayers();
        }

        public void RemovePlayer(Player player) {
            Contract.Requires(player != null && GamePlayers.ContainsKey(player) && !Started);
            var index = GamePlayers[player];
            if(index + 1 < Players.Count) {
                for (var i = index + 1; i < Players.Count; i++) {
                    GamePlayers[Players[i]]--;
                }
            }
            GamePlayers.Remove(player);
            this.UpdatePlayers();
        }

        public void ClearPlayers() {
            Contract.Requires(!Started);
            GamePlayers.Clear();
            this.UpdatePlayers();
        }

        private void UpdatePlayers() {
            Contract.Requires(!Started);
            PlayerNames.Clear();
            foreach(var player in GamePlayers.Keys) {
                PlayerNames.Add(player.Name, player);
            }
            Players = GamePlayers.Keys.ToList().AsReadOnly();
        }

        public Player GetPlayer(string playerName) {
            Contract.Requires(playerName != null && PlayerNames.ContainsKey(playerName));
            Contract.Ensures(Contract.Result<Player>() != null);
            return PlayerNames[playerName];
        }

        public bool Contains(Player player) {
            Contract.Requires(player != null);
            return GamePlayers.ContainsKey(player);
        }

        public bool Contains(string playerName) {
            Contract.Requires(playerName != null);
            return PlayerNames.ContainsKey(playerName);
        }

        public int PlayerIndex(Player player) {
            Contract.Requires(player != null && GamePlayers.ContainsKey(player));
            Contract.Ensures(Contract.Result<int>() >= 0);
            return GamePlayers[player];
        }

        public int PlayerIndex(string playerName) {
            Contract.Requires(playerName != null);
            return this.PlayerIndex(this.GetPlayer(playerName));
        }

        public Round GetRound(int round) {
            Contract.Requires(round >= 1 && round <= RoundsPossible && Rounds.ContainsKey(round));
            Contract.Ensures(Contract.Result<Round>() != null);
            return Rounds[round];
        }

        public Dictionary<Player, int> GetRoundScore(int round) {
            Contract.Requires(this.Rounds.ContainsKey(round));
            Contract.Ensures(Contract.Result<Dictionary<Player, int>>() != null);
            return this.Players.ToDictionary(player => player, player => this.Rounds[round].PlayerScore(player));
        }

        public Dictionary<Player, int> GetRoundScoreTotal(int roundNum) {
            Contract.Requires(this.Rounds.ContainsKey(roundNum));
            Contract.Ensures(Contract.Result<Dictionary<Player, int>>() != null);
            return this.Players.ToDictionary(player => player, player => this.Rounds.Where(kvp => kvp.Key <= roundNum).Sum(round => round.Value.PlayerScore(player)));
        }

        public Dictionary<Player, int> GetTotalScores() {
            Contract.Ensures(Contract.Result<Dictionary<Player, int>>() != null);
            return this.Players.ToDictionary(player => player, player => this.Rounds.Values.Sum(round => round.PlayerScore(player)));
        }
    }
}
