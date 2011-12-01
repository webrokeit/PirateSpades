using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.GameLogicV2 {
    using System.Diagnostics.Contracts;

    public class Game {
        public IList<Player> Players { get; private set; }
        private Dictionary<Player, int> GamePlayers { get; set; }
        private Dictionary<String, Player> PlayerNames { get; set; }

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
                Contract.Requires(CurrentRound >= 1 && CurrentRound <= RoundsPossible);
                Contract.Ensures(Contract.Result<Round>() == Rounds[CurrentRound]);
                return this.GetRound(CurrentRound);
            }
        }

        public bool Active {
            get {
                return CurrentRound >= 1 && CurrentRound <= Round.RoundsPossible(Players.Count) && Round != null;
            }
        }

        public delegate void GameEventDelegate(Game game);

        public event GameEventDelegate GameFinished;

        public Game() {
            GamePlayers = new Dictionary<Player, int>();
            this.UpdatePlayers();
            PlayerNames = new Dictionary<string, Player>();
            Rounds = new Dictionary<int, Round>();
            CurrentRound = 0;
        }

        public Game(IEnumerable<Player> players) : this() {
            this.AddPlayers(players);
        }

        public void Start(int startingPlayerIndex) {
            Contract.Requires(startingPlayerIndex >= 0 && startingPlayerIndex < Players.Count);
            this.NewRound(startingPlayerIndex);
        }

        public void Start(string startingPlayerName) {
            Contract.Requires(startingPlayerName != null && PlayerNames.ContainsKey(startingPlayerName));
            this.Start(PlayerNames[startingPlayerName]);
        }

        public void Start(Player startingPlayer) {
            Contract.Requires(startingPlayer != null && GamePlayers.ContainsKey(startingPlayer));
            this.Start(GamePlayers[startingPlayer]);
        }

        private void Finished() {
            if(GameFinished != null) GameFinished(this);
        }

        private void NewRound(int startingPlayerIndex) {
            Contract.Requires(startingPlayerIndex >= 0 && startingPlayerIndex < Players.Count);
            CurrentRound++;
            if(Active) {
                var r = new Round(this, startingPlayerIndex);
                r.RoundFinished += this.OnRoundFinished;
                Rounds.Add(CurrentRound, r);
                r.Start();
            } else {
                this.Finished();
            }
        }

        private void OnRoundFinished(Round round) {
            Contract.Requires(round != null);
            round.RoundFinished -= this.OnRoundFinished;
            this.NewRound((round.Dealer + 1) % Players.Count);
        }

        public void AddPlayer(Player player) {
            Contract.Requires(player != null && !GamePlayers.ContainsKey(player));
            GamePlayers.Add(player, GamePlayers.Count);
            this.UpdatePlayers();
        }

        public void AddPlayers(IEnumerable<Player> players ) {
            Contract.Requires(players != null);
            var playersBefore = GamePlayers.Count;
            foreach (var player in players.Where(player => !this.GamePlayers.ContainsKey(player))) {
                GamePlayers.Add(player, GamePlayers.Count);
            }
            if(GamePlayers.Count > playersBefore) this.UpdatePlayers();
        }

        public void RemovePlayer(Player player) {
            Contract.Requires(player != null && GamePlayers.ContainsKey(player));
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
            GamePlayers.Clear();
            this.UpdatePlayers();
        }

        private void UpdatePlayers() {
            PlayerNames.Clear();
            var l = new List<Player>(GamePlayers.Count);
            foreach(var kvp in GamePlayers) {
                PlayerNames.Add(kvp.Key.Name, kvp.Key);
                l[kvp.Value] = kvp.Key;
            }
            Players = l.AsReadOnly();
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
            return GamePlayers[player];
        }

        public Round GetRound(int round) {
            Contract.Requires(round >= 1 && round <= RoundsPossible && Rounds.ContainsKey(round));
            return Rounds[round];
        }

        public Dictionary<Player, int> GetRoundScore(int round) {
            Contract.Requires(this.Rounds.ContainsKey(round));
            return this.Players.ToDictionary(player => player, player => this.Rounds[round].PlayerScore(player));
        }

        public Dictionary<Player, int> GetTotalScores() {
            return this.Players.ToDictionary(player => player, player => this.Rounds.Values.Sum(round => round.PlayerScore(player)));
        }
    }
}
