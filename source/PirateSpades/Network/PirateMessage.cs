// <copyright file="PirateMessage.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      Message to be send between PirateHost and PirateClient.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    using PirateSpades.GameLogic;

    /// <summary>
    /// Message to be send between PirateHost and PirateClient.
    /// </summary>
    public class PirateMessage {
        /// <summary>
        /// The size of the buffer.
        /// </summary>
        public static int BufferSize = 4096;

        /// <summary>
        /// The head of the message.
        /// </summary>
        public PirateMessageHead Head { get; set; }

        /// <summary>
        /// The body of the message.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="head">The head to use.</param>
        /// <param name="body">The body of the message</param>
        public PirateMessage(string head, string body) : this(GetHead(head), body) {
            Contract.Requires(head != null && body != null);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="head">The head of the message.</param>
        /// <param name="body">The body of the message.</param>
        public PirateMessage(PirateMessageHead head, string body) {
            Contract.Requires(body != null);
            this.Head = head;
            this.Body = body;
        }

        /// <summary>
        /// Get a list of messages from a byte buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="readLen">Amount of bytes that have been read and stored in the buffer.</param>
        /// <returns>The messages parsed from the buffer.</returns>
        [Pure]
        public static List<PirateMessage> GetMessages(byte[] buffer, int readLen) {
            Contract.Requires(buffer != null && readLen <= buffer.Length && readLen > 4);
            var messages = new List<PirateMessage>(); 
            
            try {
                var data = Encoding.UTF8.GetString(buffer, 0, readLen);
                if(data.Length > 4) {
                    var start = 0;
                    while(start < data.Length) {
                        var len = 0;
                        if(int.TryParse(data.Substring(start, 4), out len)) {
                            if(len >= 4) {
                                var head = data.Substring(start + 4, 4);
                                var body = data.Substring(start + 8, len - 4);

                                messages.Add(new PirateMessage(head, body));
                            }
                            start += len + 4;
                        } else {
                            break;
                        }
                    }
                }
            } catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }

            return messages;
        }

        /// <summary>
        /// Get a PirateMessageHead from a string.
        /// </summary>
        /// <param name="head">The string to parse to a PirateMessageHead.</param>
        /// <returns>The parsed PirateMessageHead.</returns>
        [Pure]
        private static PirateMessageHead GetHead(string head) {
            PirateMessageHead pmh;
            return Enum.TryParse(head, true, out pmh) ? pmh : PirateMessageHead.Fail;
        }

        /// <summary>
        /// Get an byte array corresponding to the message.
        /// </summary>
        /// <returns>The message as a byte array.</returns>
        [Pure]
        public byte[] GetBytes() {
            var tmp = Encoding.UTF8.GetBytes(Head.ToString().ToUpper() + Body);
            var size = Encoding.UTF8.GetBytes(String.Format("{0:d4}", tmp.Length));
            var msg = new byte[4 + tmp.Length];
            size.CopyTo(msg, 0);
            tmp.CopyTo(msg, 4);
            return msg;
        }

        /// <summary>
        /// Get a PirateError from a string.
        /// </summary>
        /// <param name="s">The string to parse to a PirateError.</param>
        /// <returns>The parsed PirateError.</returns>
        [Pure]
        public static PirateError GetError(string s) {
            PirateError err;
            return Enum.TryParse(s, true, out err) ? err : PirateError.Unknown;
        }

        /// <summary>
        /// Transforms a sequence of strings into one string, separated by newlines.
        /// </summary>
        /// <param name="inputs">The sequence of strings to transform.</param>
        /// <returns>The transformed string.</returns>
        [Pure]
        public static string ConstructBody(params string[] inputs) {
            Contract.Requires(inputs != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return string.Join("\n", inputs);
        }

        /// <summary>
        /// Transforms a sequence of strings into one string, separated by newlines.
        /// </summary>
        /// <param name="inputs">The sequence of strings to transform.</param>
        /// <returns>The transformed string.</returns>
        [Pure]
        public static string ConstructBody(IEnumerable<string> inputs) {
            Contract.Requires(inputs != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return ConstructBody(inputs.ToArray());
        }

        /// <summary>
        /// Transforms a sequence of strings into one string, separated by newlines.
        /// </summary>
        /// <param name="body">The body to append the transformed string to.</param>
        /// <param name="inputs">The sequence of strings to transform.</param>
        /// <returns>The transformed string.</returns>
        [Pure]
        public static string AppendBody(string body, params string[] inputs) {
            Contract.Requires(body != null && inputs != null);
            Contract.Ensures(Contract.Result<string>() != null);
            if(string.IsNullOrEmpty(body)) {
                return ConstructBody(inputs);
            }
            return body + "\n" + ConstructBody(inputs);
        }

        /// <summary>
        /// Construct Host Information.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructHostInfo(PirateHost host) {
            Contract.Requires(host != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return ConstructBody(
                ConstructHostIp(host),
                ConstructGameName(host),
                ConstructPlayersInGame(host.PlayerCount),
                ConstructMaxPlayersInGame(host.MaxPlayers));
        }

        /// <summary>
        /// Construct Host IP.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructHostIp(PirateHost host) {
            Contract.Requires(host != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "host_ip: " + host.Ip;
        }

        /// <summary>
        /// Get the Host Ip.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns></returns>
        [Pure]
        public static IPAddress GetHostIp(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^host_ip: [0-9.]+$", RegexOptions.Multiline));
            Contract.Requires(PirateScanner.IsValidIp(Regex.Match(msg.Body, @"^host_ip: ([0-9.]+)$", RegexOptions.Multiline).Groups[1].Value));
            Contract.Ensures(Contract.Result<IPAddress>() != null);
            return PirateScanner.GetIp(Regex.Match(msg.Body, @"^host_ip: ([0-9.]+)$", RegexOptions.Multiline).Groups[1].Value);
        }

        /// <summary>
        /// Construct Game Name.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructGameName(PirateHost host) {
            Contract.Requires(host != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "game_name: " + host.GameName;
        }

        /// <summary>
        /// Get the Game Name.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>The Get Name.</returns>
        [Pure]
        public static string GetGameName(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^game_name: .+$", RegexOptions.Multiline));
            Contract.Requires(PirateHost.IsValidGameName(Regex.Match(msg.Body, @"^game_name: (.+)$", RegexOptions.Multiline).Groups[1].Value));
            Contract.Ensures(Contract.Result<string>() != null);
            return Regex.Match(msg.Body, @"^game_name: (.+)$", RegexOptions.Multiline).Groups[1].Value;
        }

        /// <summary>
        /// Construct Player Name.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructPlayerName(Player player) {
            Contract.Requires(player != null && player.Name != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return ConstructPlayerName(player.Name);
        }

        /// <summary>
        /// Construct Player Name.
        /// </summary>
        /// <param name="name">The player name.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructPlayerName(string name) {
            Contract.Requires(name != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "player_name: " + name;
        }

        /// <summary>
        /// Get the Player Name.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>The Player Name.</returns>
        [Pure]
        public static string GetPlayerName(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^player_name: ([a-zA-Z0-9]{3,12})$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<string>() != null);
            return Regex.Match(msg.Body, @"^player_name: ([a-zA-Z0-9]{3,12})$", RegexOptions.Multiline).Groups[1].Value;
        }

        /// <summary>
        /// Get a set of Player Names.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>A set of Player Names.</returns>
        [Pure]
        public static HashSet<string> GetPlayerNames(PirateMessage msg) {
            Contract.Requires(msg != null);
            Contract.Ensures(Contract.Result<HashSet<string>>() != null);
            var res = new HashSet<string>();
            foreach(Match m in Regex.Matches(msg.Body, @"^player_name: ([a-zA-Z0-9]{3,12})$", RegexOptions.Multiline)) {
                res.Add(m.Groups[1].Value);
            }
            return res;
        }

        /// <summary>
        /// Construct Players In Game.
        /// </summary>
        /// <param name="players">The amount of players in game.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructPlayersInGame(int players) {
            Contract.Requires(players >= 0);
            Contract.Ensures(Contract.Result<string>() != null);
            return "players_ingame: " + players;
        }
        
        /// <summary>
        /// Get amount of players in game.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>Amount of players in game.</returns>
        [Pure]
        public static int GetPlayersInGame(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^players_ingame: [0-" + Game.MaxPlayersInGame + "]$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= Game.MaxPlayersInGame);
            return
                int.Parse(
                    Regex.Match(msg.Body, @"^players_ingame: ([0-" + Game.MaxPlayersInGame + "])$", RegexOptions.Multiline).Groups[1].Value);
        }

        /// <summary>
        /// Construct Max Players In Game
        /// </summary>
        /// <param name="players">Max amount of players in game.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructMaxPlayersInGame(int players) {
            Contract.Requires(players >= 0);
            Contract.Ensures(Contract.Result<string>() != null);
            return "players_ingamemax: " + players;
        }

        /// <summary>
        /// Get max amount of players allowed in game.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>Max amount of players allowed in game</returns>
        [Pure]
        public static int GetMaxPlayersInGame(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^players_ingamemax: [0-" + Game.MaxPlayersInGame + "]$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= Game.MaxPlayersInGame);
            return
                int.Parse(
                    Regex.Match(msg.Body, @"^players_ingamemax: ([0-" + Game.MaxPlayersInGame + "])$", RegexOptions.Multiline).Groups[1].Value);
        }

        /// <summary>
        /// Construct Player Bet.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructPlayerBet(Player player) {
            Contract.Requires(player != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "player_bet: " + player.Name + ";" + player.Bet;
        }

        /// <summary>
        /// Get the bets of the players.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>A dictionary of player names and the amount they betted.</returns>
        [Pure]
        public static Dictionary<string, int> GetPlayerBets(PirateMessage msg) {
            Contract.Requires(msg != null);
            Contract.Ensures(Contract.Result<Dictionary<string, int>>() != null);
            var res = new Dictionary<string, int>();
            foreach(Match m in Regex.Matches(msg.Body, @"^player_bet: ([a-zA-Z0-9]+);(10|[0-9])$", RegexOptions.Multiline)) {
                res[m.Groups[1].Value] = int.Parse(m.Groups[2].Value);
            }
            return res;
        }

        /// <summary>
        /// Construct Round Number.
        /// </summary>
        /// <param name="round">The round.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructRoundNumber(int round) {
            Contract.Requires(round >= 1 && round <= 20);
            Contract.Ensures(Contract.Result<string>() != null);
            return "round: " + round;
        }

        /// <summary>
        /// Get the Round Number.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>The Round Number</returns>
        [Pure]
        public static int GetRound(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^round: ([1-9]|1[0-9]|20)$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<int>() >= 1 && Contract.Result<int>() <= 20);
            return
                int.Parse(Regex.Match(msg.Body, @"^round: ([1-9]|1[0-9]|20)$", RegexOptions.Multiline).Groups[1].Value);
        }

        /// <summary>
        /// Construct Dealer.
        /// </summary>
        /// <param name="name">The name of the dealer.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructDealer(string name) {
            Contract.Requires(name != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "dealer: " + name;
        }

        /// <summary>
        /// Get the Dealer.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>The Dealer.</returns>
        [Pure]
        public static string GetDealer(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^dealer: [a-zA-Z0-9]+$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<string>() != null);
            return Regex.Match(msg.Body, @"^dealer: ([a-zA-Z0-9]+)$", RegexOptions.Multiline).Groups[1].Value;
        }

        /// <summary>
        /// Construct Starting Player-
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructStartingPlayer(Player player) {
            Contract.Requires(player != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "starting_player: " + player.Name;
        }

        /// <summary>
        /// Get the name of Starting Player.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>The name of the Starting Player.</returns>
        [Pure]
        public static string GetStartingPlayer(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^starting_player: [a-zA-Z0-9]+$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<string>() != null);
            return Regex.Match(msg.Body, @"^starting_player: ([a-zA-Z0-9]+)$", RegexOptions.Multiline).Groups[1].Value;
        }

        /// <summary>
        /// Construct Winner.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructWinner(Player player) {
            Contract.Requires(player != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "winning_player: " + player.Name;
        }

        /// <summary>
        /// Get the name of the Winner.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>The name of the Winner.</returns>
        [Pure]
        public static string GetWinner(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^winning_player: [a-zA-Z0-9]\w+$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<string>() != null);
            return Regex.Match(msg.Body, @"^winning_player: ([a-zA-Z0-9]+)$", RegexOptions.Multiline).Groups[1].Value;
        }

        /// <summary>
        /// Construct Player Trick.
        /// </summary>
        /// <param name="round">The round.</param>
        /// <param name="player">The player.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructPlayerTrick(Round round, Player player) {
            Contract.Requires(round != null && player != null && round.PlayerTricks.ContainsKey(player));
            Contract.Ensures(Contract.Result<string>() != null);
            return "player_tricks: " + player.Name + ";" + round.PlayerTricks[player].Count;
        }

        /// <summary>
        /// Construct Player Tricks.
        /// </summary>
        /// <param name="round">The round.</param>
        /// <returns>A list of strings to be used when sending messages.</returns>
        [Pure]
        public static IList<string> ConstructPlayerTricks(Round round) {
            Contract.Requires(round != null);
            Contract.Ensures(Contract.Result<IList<string>>() != null);
            return round.PlayerTricks.Select(kvp => ConstructPlayerTrick(round, kvp.Key)).ToList();
        }

        /// <summary>
        /// Get a dictionary of player names and the amount of tricks they have.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>A dictionary of player names and the amount of tricks they have.</returns>
        [Pure]
        public static Dictionary<string, int> GetPlayerTricks(PirateMessage msg) {
            Contract.Requires(msg != null);
            Contract.Ensures(Contract.Result<Dictionary<string, int>>() != null);
            var res = new Dictionary<string, int>();
            foreach(Match m in Regex.Matches(msg.Body, @"^player_tricks: ([a-zA-Z0-9]+);(10|[0-9]+)$", RegexOptions.Multiline)) {
                res[m.Groups[1].Value] = int.Parse(m.Groups[2].Value);
            }
            return res;
        }

        /// <summary>
        /// Construct Player score.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="score">The score.</param>
        /// <returns>A string to be used when sending messages.</returns>
        [Pure]
        public static string ConstructPlayerScore(Player player, int score) {
            Contract.Requires(player != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "player_score: " + player.Name + ";" + score;
        }

        /// <summary>
        /// Construct Player Scores.
        /// </summary>
        /// <param name="scores">A dictionary of players and their scores.</param>
        /// <returns>A list of strings to be used when sending messages.</returns>
        [Pure]
        public static IList<string> ContstructPlayerScores(Dictionary<Player, int> scores) {
            Contract.Requires(scores != null);
            Contract.Ensures(Contract.Result<IList<string>>() != null);
            return scores.Select(kvp => ConstructPlayerScore(kvp.Key, kvp.Value)).ToList();
        }

        /// <summary>
        /// Get a dictionary of player names and their current game score.
        /// </summary>
        /// <param name="msg">The message to get from.</param>
        /// <returns>A dictionary of player names and their current game score.</returns>
        [Pure]
        public static Dictionary<string, int> GetPlayerScores(PirateMessage msg) {
            Contract.Requires(msg != null);
            Contract.Ensures(Contract.Result<Dictionary<string, int>>() != null);
            var res = new Dictionary<string, int>();
            foreach(Match m in Regex.Matches(msg.Body, @"^player_score: ([a-zA-Z0-9]+);(-?[0-9]+)$", RegexOptions.Multiline)) {
                res[m.Groups[1].Value] = int.Parse(m.Groups[2].Value);
            }
            return res;
        }
    }

    public enum PirateMessageHead {
        /// <summary>Failure</summary>
        Fail, // Failure

        /// <summary>Error</summary>
        Erro,

        /// <summary>Knock Knock (For scanning)</summary>
        Knck,

        /// <summary>Broadcast</summary>
        Bcst,

        /// <summary>Init Player Connection</summary>
        Init,

        /// <summary>Verify</summary>
        Verf,

        /// <summary>Player Information</summary>
        Pnfo,

        /// <summary>Players In Game</summary>
        Pigm, 

        /// <summary>Player Accept</summary>
        Pacp,

        /// <summary>Game Started</summary>
        Gstr,

        /// <summary>Game Finished</summary>
        Gfin,

        /// <summary>Transfer Card</summary>
        Xcrd,

        /// <summary>Play Card</summary>
        Pcrd,

        /// <summary>Trick Done</summary>
        Trdn,

        /// <summary>Player Bet</summary>
        Pbet,

        /// <summary>New Round</summary>
        Nrnd,

        /// <summary>Begin Round</summary>
        Bgrn,

        /// <summary>Finish Round</summary>
        Frnd,

        /// <summary>Done Dealing Cards</summary>
        Ddlc,

        /// <summary>Request Bets</summary>
        Breq,

        /// <summary>Request Card</summary>
        Creq
    }

    public enum PirateError {
        /// <summary>Unknown error. Also used if error send could not be identified.</summary>
        Unknown,

        /// <summary>You're already connected.</summary>
        AlreadyConnected,

        /// <summary>No new connection allowed.</summary>
        NoNewConnections,

        /// <summary>The name the user wanted to use was already taken.</summary>
        NameAlreadyTaken,

        /// <summary>Invalid bet.</summary>
        InvalidBet,

        /// <summary>Card not playable.</summary>
        CardNotPlayable
    }
}