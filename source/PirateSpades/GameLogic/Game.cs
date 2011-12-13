// <copyright file="Game.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A representation of a game in the Pirate Spades Game.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.GameLogic {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics.Contracts;

    using PirateSpades.Misc;

    /// <summary>
    /// A representation of a game in the Pirate Spades Game.
    /// </summary>
    public class Game {
        /// <summary>
        /// The minimum amount of players in a game before it can start.
        /// </summary>
        public static readonly int MinPlayersInGame = 2;

        /// <summary>
        /// The maximum amount of lpayers in a game.
        /// </summary>
        public static readonly int MaxPlayersInGame = 5;

        /// <summary>
        /// Players in the game.
        /// </summary>
        public IList<Player> Players { get; private set; }

        /// <summary>
        /// Players in the game and their corresponding index in the Players list.
        /// </summary>
        private OrderedDictionary<Player, int> GamePlayers { get; set; }

        /// <summary>
        /// Player names and their corresponding player.
        /// </summary>
        private Dictionary<String, Player> PlayerNames { get; set; }

        /// <summary>
        /// The index of the Current Dealer.
        /// </summary>
        public int CurrentDealer { get; private set; }

        /// <summary>
        /// Amount of cards to deal per player.
        /// </summary>
        public int CardsToDeal{
            get {
                Contract.Requires(CurrentRound > 0 && Players.Count > 0);
                return Card.CardsToDeal(CurrentRound, Players.Count);
            }
        }

        /// <summary>
        /// Amount of rounds possible.
        /// </summary>
        public int RoundsPossible {
            get {
                Contract.Requires(Players.Count > 0);
                return Round.RoundsPossible(Players.Count);
            }
        }

        /// <summary>
        /// List of rounds in the game.
        /// </summary>
        public IList<Round> Rounds { get; set; }

        /// <summary>
        /// The current round number.
        /// </summary>
        public int CurrentRound { get; private set; }
        
        /// <summary>
        /// The current round being played.
        /// </summary>
        public Round Round {
            get {
                Contract.Requires(Started && CurrentRound >= 1 && CurrentRound <= RoundsPossible && Players.Count >= MinPlayersInGame);
                Contract.Ensures(Contract.Result<Round>() == Rounds[CurrentRound - 1]);
                return this.GetRound(CurrentRound);
            }
        }
        
        /// <summary>
        /// Whether or not the game has been started.
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        /// Whether ot not the game is active.
        /// </summary>
        public bool Active {
            get {
                Contract.Requires(Players.Count >= MinPlayersInGame);
                return Started && CurrentRound >= 1 && CurrentRound <= RoundsPossible && !Finished;
            }
        }

        /// <summary>
        /// Whether or not the game has finished.
        /// </summary>
        public bool Finished {
            get {
                Contract.Requires(Players.Count >= MinPlayersInGame);
                return Started && CurrentRound == RoundsPossible && CurrentRound == Rounds.Count && Round.Finished;
            }
        }

        /// <summary>
        /// The current game leader.
        /// </summary>
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

        /// <summary>
        /// Whether or not the game was created by a host.
        /// </summary>
        private bool IsHost { get; set; }

        /// <summary>
        /// Delegate to be used for events involving the game.
        /// </summary>
        /// <param name="game"></param>
        public delegate void GameEventDelegate(Game game);

        /// <summary>
        /// Fires when a round has been started.
        /// </summary>
        public event GameEventDelegate RoundStarted;

        /// <summary>
        /// Fires when a round has begun.
        /// </summary>
        public event GameEventDelegate RoundBegun;

        /// <summary>
        /// Fires when a round has finished.
        /// </summary>
        public event GameEventDelegate RoundFinished;

        /// <summary>
        /// Fires when a new pile has been created.
        /// </summary>
        public event GameEventDelegate RoundNewPile;

        /// <summary>
        /// Fires when the game has finished.
        /// </summary>
        public event GameEventDelegate GameFinished;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Game() {
            GamePlayers = new OrderedDictionary<Player, int>();
            PlayerNames = new Dictionary<string, Player>();
            this.UpdatePlayers();
            Rounds = new List<Round>().AsReadOnly();
            CurrentRound = 0;
            Started = false;
            IsHost = false;
            CurrentDealer = -1;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="players">Players to add to the game.</param>
        public Game(IEnumerable<Player> players) : this() {
            Contract.Requires(players != null);
            this.AddPlayers(players);
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        /// <param name="dealer">The dealer for the first round.</param>
        public void Start(int dealer) {
            Contract.Requires(Players.Count >= MinPlayersInGame && dealer >= 0 && dealer < Players.Count);
            this.Start(false, dealer);
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        /// <param name="isHost">Whether or not the game is being started by the host.</param>
        /// <param name="dealer">The dealer for the first round.</param>
        public void Start(bool isHost, int dealer) {
            Contract.Requires(Players.Count >= MinPlayersInGame && dealer >= 0 && dealer < Players.Count);
            CurrentDealer = dealer;
            this.Started = true;
            this.IsHost = isHost;

            var lRounds = new List<Round>();
            for (var i = 1; i <= RoundsPossible; i++) {
                lRounds.Add(new Round(this, i, dealer));
                dealer = (dealer + 1) % Players.Count;
            }
            Rounds = lRounds.AsReadOnly();

            if (this.IsHost) {
                this.NewRound();
            }
        }

        /// <summary>
        /// Finish the game.
        /// </summary>
        private void Finish() {
            Contract.Requires(Finished);
            if(GameFinished != null) GameFinished(this);
        }

        /// <summary>
        /// Start a new round.
        /// </summary>
        public void NewRound() {
            Contract.Requires(Started);
            CurrentRound++;
            if(Active) {
                var r = GetRound(CurrentRound);
                r.RoundBegun += this.OnRoundBegun;
                r.RoundFinished += this.OnRoundFinished;
                r.NewPile += this.OnRoundNewPile;

                r.Start();
                if (RoundStarted != null) RoundStarted(this);
            } else {
                CurrentRound--;
                this.Finish();
            }
        }

        /// <summary>
        /// A new round has begun.
        /// </summary>
        /// <param name="round">The new round.</param>
        private void OnRoundBegun(Round round) {
            Contract.Requires(round != null && Active);
            if (RoundBegun != null) RoundBegun(this);
            round.RoundBegun -= this.OnRoundBegun;
        }

        /// <summary>
        /// A round has finished.
        /// </summary>
        /// <param name="round">The finished round.</param>
        private void OnRoundFinished(Round round) {
            Contract.Requires(round != null && (Active || Finished));
            if (RoundFinished != null) RoundFinished(this);
            round.RoundFinished -= this.OnRoundFinished;
            round.NewPile -= this.OnRoundNewPile;
        }

        /// <summary>
        /// A new pile has been created.
        /// </summary>
        /// <param name="round">The round with the new pile.</param>
        private void OnRoundNewPile(Round round) {
            if (RoundNewPile != null) RoundNewPile(this);
        }

        /// <summary>
        /// Add a player to the game.
        /// </summary>
        /// <param name="player">The player.</param>
        public void AddPlayer(Player player) {
            Contract.Requires(player != null && !GamePlayers.ContainsKey(player) && !Started);
            GamePlayers.Add(player, GamePlayers.Count);
            this.UpdatePlayers();
        }

        /// <summary>
        /// Add multiple players to the game.
        /// </summary>
        /// <param name="players">The players.</param>
        public void AddPlayers(IEnumerable<Player> players) {
            Contract.Requires(players != null && !Started);
            var playersBefore = GamePlayers.Count;
            foreach (var player in players.Where(player => player != null && !this.GamePlayers.ContainsKey(player))) {
                GamePlayers.Add(player, GamePlayers.Count);
            }
            if(GamePlayers.Count > playersBefore) this.UpdatePlayers();
        }

        /// <summary>
        /// Remove a player from the game.
        /// </summary>
        /// <param name="player">The player to remove.</param>
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

        /// <summary>
        /// Clear the players.
        /// </summary>
        public void ClearPlayers() {
            Contract.Requires(!Started);
            GamePlayers.Clear();
            this.UpdatePlayers();
        }

        /// <summary>
        /// Update the players.
        /// </summary>
        private void UpdatePlayers() {
            Contract.Requires(!Started);
            PlayerNames.Clear();
            foreach(var player in GamePlayers.Keys) {
                PlayerNames.Add(player.Name, player);
            }
            Players = GamePlayers.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        /// Get a player from a player name.
        /// </summary>
        /// <param name="playerName">The player name.</param>
        /// <returns>The player with the specified player name.</returns>
        public Player GetPlayer(string playerName) {
            Contract.Requires(playerName != null && PlayerNames.ContainsKey(playerName));
            Contract.Ensures(Contract.Result<Player>() != null);
            return PlayerNames[playerName];
        }

        /// <summary>
        /// Checks whether or not the specified player is in the game.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>True if the specified player is in the game, false if not.</returns>
        public bool Contains(Player player) {
            Contract.Requires(player != null);
            return GamePlayers.ContainsKey(player);
        }

        /// <summary>
        /// Checks whether or not the specified player is in the game.
        /// </summary>
        /// <param name="playerName">The player name</param>
        /// <returns>True if the specified player is in the game, false if not.</returns>
        public bool Contains(string playerName) {
            Contract.Requires(playerName != null);
            return PlayerNames.ContainsKey(playerName);
        }

        /// <summary>
        /// Get the index of the player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The index of the player.</returns>
        public int PlayerIndex(Player player) {
            Contract.Requires(player != null && GamePlayers.ContainsKey(player));
            Contract.Ensures(Contract.Result<int>() >= 0);
            return GamePlayers[player];
        }

        /// <summary>
        /// Get the index of the player.
        /// </summary>
        /// <param name="playerName">The player.</param>
        /// <returns>The index of the player.</returns>
        public int PlayerIndex(string playerName) {
            Contract.Requires(playerName != null && PlayerNames.ContainsKey(playerName));
            Contract.Ensures(Contract.Result<int>() >= 0);
            return this.PlayerIndex(this.GetPlayer(playerName));
        }

        /// <summary>
        /// Get the round with the specified number.
        /// </summary>
        /// <param name="round">The round number.</param>
        /// <returns>The round with the specified number.</returns>
        public Round GetRound(int round) {
            Contract.Requires(Started && round >= 1 && round <= RoundsPossible && Rounds.Count >= round);
            Contract.Ensures(Contract.Result<Round>() != null);
            return Rounds[round - 1];
        }

        /// <summary>
        /// Get the scores of a specific round.
        /// </summary>
        /// <param name="round">The round number.</param>
        /// <returns>A dictionary containing the players and their round score.</returns>
        public Dictionary<Player, int> GetRoundScore(int round) {
            Contract.Requires(Started && round >= 1 && round <= RoundsPossible);
            Contract.Ensures(Contract.Result<Dictionary<Player, int>>() != null);
            return this.Players.ToDictionary(player => player, player => this.GetRound(round).PlayerScore(player));
        }

        /// <summary>
        /// Get the total scores up until the specified round.
        /// </summary>
        /// <param name="roundNum">The round number.</param>
        /// <returns>A dictionary containing the players and their total score up until the specified round.</returns>
        public Dictionary<Player, int> GetRoundScoreTotal(int roundNum) {
            Contract.Requires(Started && roundNum >= 1 && roundNum <= RoundsPossible);
            Contract.Ensures(Contract.Result<Dictionary<Player, int>>() != null);
            return this.Players.ToDictionary(player => player, player => this.Rounds.Where(round => round.Number <= roundNum).Sum(round => round.PlayerScore(player)));
        }

        /// <summary>
        /// Get the total scores up until the current round.
        /// </summary>
        /// <returns>A dictionary containing the players and their total score up until the current round.</returns>
        public Dictionary<Player, int> GetTotalScores() {
            Contract.Requires(Started && CurrentRound >= 1 && CurrentRound <= RoundsPossible);
            Contract.Ensures(Contract.Result<Dictionary<Player, int>>() != null);
            return this.GetRoundScoreTotal(CurrentRound);
        }

        /// <summary>
        /// Get all the total scores for all the rounds and for all the players.
        /// </summary>
        /// <returns>A dictionary contaning the rounds, the players and their total scores for each round.</returns>
        public Dictionary<int, Dictionary<Player, int>> GetScoreTable() {
            Contract.Requires(Started);
            Contract.Ensures(Contract.Result<Dictionary<int, Dictionary<Player, int>>>() != null);
            return this.Rounds.Where(round => round.Number <= CurrentRound).ToDictionary(round => round.Number, round => this.GetRoundScoreTotal(round.Number));
        } 
    }
}
