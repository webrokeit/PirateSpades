namespace PirateSpades.GameLogic {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics.Contracts;

    using PirateSpades.Misc;

    public class Game {
        public static readonly int MinPlayersInGame = 2;
        public static readonly int MaxPlayersInGame = 5;

        public IList<Player> Players { get; private set; }
        private OrderedDictionary<Player, int> GamePlayers { get; set; }
        private Dictionary<String, Player> PlayerNames { get; set; }
        public int CurrentDealer { get; private set; }

        public int CardsToDeal{
            get {
                Contract.Requires(CurrentRound > 0 && Players.Count > 0);
                return Card.CardsToDeal(CurrentRound, Players.Count);
            }
        }

        public int RoundsPossible {
            get {
                Contract.Requires(Players.Count > 0);
                return Round.RoundsPossible(Players.Count);
            }
        }

        private Dictionary<int, Round> Rounds { get; set; }
        public int CurrentRound { get; private set; }
        public Round Round {
            get {
                Contract.Requires(Started && CurrentRound >= 1 && CurrentRound <= RoundsPossible && Players.Count >= MinPlayersInGame);
                Contract.Ensures(Contract.Result<Round>() == Rounds[CurrentRound]);
                return this.GetRound(CurrentRound);
            }
        }
        public bool Started { get; private set; }
        public bool Active {
            get {
                Contract.Requires(Players.Count >= MinPlayersInGame);
                return Started && CurrentRound >= 1 && CurrentRound <= RoundsPossible && !Finished;
            }
        }
        public bool Finished {
            get {
                Contract.Requires(Players.Count >= MinPlayersInGame);
                return Started && CurrentRound == RoundsPossible && CurrentRound == Rounds.Count && Round.Finished;
            }
        }
        public Player Leader {
            get {
                Contract.Requires(Started && CurrentRound >= 1 && Players.Count >= MinPlayersInGame);
                Contract.Ensures(Contract.Result<Player>() != null);
                var scores = this.GetTotalScores();
                Player leader = null;
                var maxScore = int.MinValue;
                foreach(var key in scores.Keys) {
                    if(leader == null || scores[key] > maxScore) {
                        leader = key;
                        maxScore = scores[key];
                    }
                }
                return leader;
            }
        }
        private bool IsHost { get; set; }

        public delegate void GameEventDelegate(Game game);

        public event GameEventDelegate RoundStarted;
        public event GameEventDelegate RoundBegun;
        public event GameEventDelegate RoundFinished;
        public event GameEventDelegate RoundNewPile;
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
            Contract.Requires(players != null);
            this.AddPlayers(players);
        }

        public void Start(int dealer) {
            Contract.Requires(Players.Count >= MinPlayersInGame && dealer >= 0 && dealer < Players.Count);
            this.Start(false, dealer);
        }

        public void Start(bool isHost, int dealer) {
            Contract.Requires(Players.Count >= MinPlayersInGame && dealer >= 0 && dealer < Players.Count);
            CurrentDealer = dealer;
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
                if (CurrentRound > 1) {
                    CurrentDealer = (this.GetRound(CurrentRound - 1).Dealer + 1) % Players.Count;
                }

                var r = new Round(this, CurrentDealer);
                if(IsHost) {
                    r.RoundBegun += this.OnRoundBegun;
                    r.RoundFinished += this.OnRoundFinished;
                    r.NewPile += this.OnRoundNewPile;
                }
                Rounds.Add(CurrentRound, r);
                r.Start();
                if (RoundStarted != null) RoundStarted(this);
            } else {
                CurrentRound--;
                this.Finish();
            }
        }

        private void OnRoundBegun(Round round) {
            Contract.Requires(round != null && Active);
            if (RoundBegun != null) RoundBegun(this);
            if(IsHost) {
                round.RoundBegun -= this.OnRoundBegun;
            }
        }

        private void OnRoundFinished(Round round) {
            Contract.Requires(round != null && (Active || Finished));
            if (RoundFinished != null) RoundFinished(this);
            if (IsHost) {
                round.RoundFinished -= this.OnRoundFinished;
                round.NewPile -= this.OnRoundNewPile;
                //this.NewRound();
            }
        }

        private void OnRoundNewPile(Round round) {
            if (RoundNewPile != null) RoundNewPile(this);
        }

        public void AddPlayer(Player player) {
            Contract.Requires(player != null && !GamePlayers.ContainsKey(player) && !Started);
            GamePlayers.Add(player, GamePlayers.Count);
            this.UpdatePlayers();
        }

        public void AddPlayers(IEnumerable<Player> players) {
            Contract.Requires(players != null && !Started);
            var playersBefore = GamePlayers.Count;
            foreach (var player in players.Where(player => player != null && !this.GamePlayers.ContainsKey(player))) {
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
            Contract.Requires(playerName != null && PlayerNames.ContainsKey(playerName));
            Contract.Ensures(Contract.Result<int>() >= 0);
            return this.PlayerIndex(this.GetPlayer(playerName));
        }

        public Round GetRound(int round) {
            Contract.Requires(Started && round >= 1);
            Contract.Requires(round <= RoundsPossible);
            Contract.Requires(Rounds.ContainsKey(round));
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

        public Dictionary<int, Dictionary<Player, int>> GetScoreTable() {
            Contract.Ensures(Contract.Result<Dictionary<int, Dictionary<Player, int>>>() != null);
            return this.Rounds.Keys.ToDictionary(round => round, this.GetRoundScoreTotal);
        } 
    }
}
