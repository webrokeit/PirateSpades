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

    using PirateSpades.GameLogicV2;

    public class PirateMessage {
        public static int BufferSize = 4096;
        public PirateMessageHead Head { get; set; }
        public string Body { get; set; }

        public PirateMessage(string head, string body) : this(GetHead(head), body) {
            Contract.Requires(head != null && body != null);
        }

        public PirateMessage(PirateMessageHead head, string body) {
            Contract.Requires(body != null);
            this.Head = head;
            this.Body = body;
        }

        public static List<PirateMessage> GetMessages(byte[] buffer, int readLen) {
            Contract.Requires(buffer != null && readLen <= buffer.Length && readLen > 4);

            var messages = new List<PirateMessage>();
            var data = Encoding.UTF8.GetString(buffer, 0, readLen);
            if(data.Length > 4) {
                var start = 0;
                while (start < data.Length) {
                    var len = 0;
                    if(int.TryParse(data.Substring(start, 4), out len)) {
                        if (len >= 4) {
                            var head = data.Substring(start + 4, 4);
                            var body = data.Substring(start + 8, len - 4);

                            messages.Add(new PirateMessage(head, body));
                        }
                        start += len + 4;
                    }else {
                        break;
                    }
                }
            }

            return messages;
        }

        private static PirateMessageHead GetHead(string head) {
            PirateMessageHead pmh;
            return Enum.TryParse(head, true, out pmh) ? pmh : PirateMessageHead.Fail;
        }

        public byte[] GetBytes() {
            var tmp = Encoding.UTF8.GetBytes(Head.ToString().ToUpper() + Body);
            var size = Encoding.UTF8.GetBytes(String.Format("{0:d4}", tmp.Length));
            var msg = new byte[4 + tmp.Length];
            size.CopyTo(msg, 0);
            tmp.CopyTo(msg, 4);
            return msg;
        }

        public static PirateError GetError(string s) {
            PirateError err;
            return Enum.TryParse(s, true, out err) ? err : PirateError.Unknown;
        }

        public static string ConstructBody(params string[] inputs) {
            Contract.Requires(inputs != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return string.Join("\n", inputs);
        }

        public static string ConstructBody(IEnumerable<string> inputs) {
            Contract.Requires(inputs != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return ConstructBody(inputs.ToArray());
        }

        public static string AppendBody(string body, params string[] inputs) {
            Contract.Requires(body != null && inputs != null);
            Contract.Ensures(Contract.Result<string>() != null);
            if(string.IsNullOrEmpty(body)) {
                return ConstructBody(inputs);
            }
            return body + "\n" + ConstructBody(inputs);
        }

        public static string ConstructHostInfo(PirateHost host) {
            Contract.Requires(host != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return ConstructBody(
                ConstructHostIp(host),
                ConstructGameName(host),
                ConstructPlayersInGame(host.PlayerCount),
                ConstructMaxPlayersInGame(host.MaxPlayers));
        }

        public static string ConstructHostIp(PirateHost host) {
            Contract.Requires(host != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "host_ip: " + host.Ip;
        }

        public static IPAddress GetHostIp(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^host_ip: [0-9.]+$", RegexOptions.Multiline));
            Contract.Requires(PirateScanner.IsValidIp(Regex.Match(msg.Body, @"^host_ip: ([0-9.]+)$", RegexOptions.Multiline).Groups[1].Value));
            Contract.Ensures(Contract.Result<IPAddress>() != null);
            return PirateScanner.GetIp(Regex.Match(msg.Body, @"^host_ip: ([0-9.]+)$", RegexOptions.Multiline).Groups[1].Value);
        }

        public static string ConstructGameName(PirateHost host) {
            Contract.Requires(host != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "game_name: " + host.GameName;
        }

        public static string GetGameName(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^game_name: .+$", RegexOptions.Multiline));
            Contract.Requires(PirateHost.IsValidGameName(Regex.Match(msg.Body, @"^game_name: (.+)$", RegexOptions.Multiline).Groups[1].Value));
            Contract.Ensures(Contract.Result<string>() != null);
            return Regex.Match(msg.Body, @"^game_name: (.+)$", RegexOptions.Multiline).Groups[1].Value;
        }

        public static string ConstructPlayerName(Player player) {
            Contract.Requires(player != null && player.Name != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return ConstructPlayerName(player.Name);
        }

        public static string ConstructPlayerName(string name) {
            Contract.Requires(name != null);
            Contract.Ensures(Contract.Result<string>() != null);
            return "player_name: " + name;
        }

        public static string GetPlayerName(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^player_name: (\w{3,20})$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<string>() != null);
            return Regex.Match(msg.Body, @"^player_name: (\w{3,20})$", RegexOptions.Multiline).Groups[1].Value;
        }

        public static HashSet<string> GetPlayerNames(PirateMessage msg) {
            Contract.Requires(msg != null);
            Contract.Ensures(Contract.Result<HashSet<string>>() != null);
            var res = new HashSet<string>();
            foreach (Match m in Regex.Matches(msg.Body, @"^player_name: (\w{3,20})$", RegexOptions.Multiline)) {
                res.Add(m.Groups[1].Value);
            }
            return res;
        }

        public static string ConstructPlayersInGame(int players) {
            Contract.Requires(players >= 0);
            Contract.Ensures(Contract.Result<string>() != null);
            return "players_ingame: " + players;
        }

        public static int GetPlayersInGame(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^players_ingame: [0-5]$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= Game.MinPlayersInGame);
            return
                int.Parse(
                    Regex.Match(msg.Body, @"^players_ingame: ([0-5])$", RegexOptions.Multiline).Groups[1].Value);
        }

        public static string ConstructMaxPlayersInGame(int players) {
            Contract.Requires(players >= 0);
            Contract.Ensures(Contract.Result<string>() != null);
            return "players_ingamemax: " + players;
        }

        public static int GetMaxPlayersInGame(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^players_ingamemax: [0-5]$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= Game.MinPlayersInGame);
            return
                int.Parse(
                    Regex.Match(msg.Body, @"^players_ingamemax: ([0-5])$", RegexOptions.Multiline).Groups[1].Value);
        }

        public static string ConstructPlayerBet(Player player) {
            Contract.Requires(player != null);
            return "player_bet: " + player.Name + ";" + player.Bet;
        }

        public static Dictionary<string, int> GetPlayerBets(PirateMessage msg) {
            Contract.Requires(msg != null);
            Contract.Ensures(Contract.Result<Dictionary<string, int>>() != null);
            var res = new Dictionary<string, int>();
            foreach(Match m in Regex.Matches(msg.Body, @"^player_bet: (\w+);([0-9])$", RegexOptions.Multiline)) {
                res[m.Groups[1].Value] = int.Parse(m.Groups[2].Value);
            }
            return res;
        }

        public static string ConstructRoundNumber(int round) {
            Contract.Requires(round >= 1 && round <= 20);
            return "round: " + round;
        }

        public static int GetRound(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^round: ([1-9]|1[0-9]|20)$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<int>() >= 1 && Contract.Result<int>() <= 20);
            return
                int.Parse(Regex.Match(msg.Body, @"^round: ([1-9]|1[0-9]|20)$", RegexOptions.Multiline).Groups[1].Value);
        }

        public static string ConstructDealer(string name) {
            Contract.Requires(name != null);
            return "dealer: " + name;
        }

        public static string GetDealer(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^dealer: \w+$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<string>() != null);
            return Regex.Match(msg.Body, @"^dealer: (\w+)$", RegexOptions.Multiline).Groups[1].Value;
        }

        public static string ConstructStartingPlayer(Player player) {
            Contract.Requires(player != null);
            return "starting_player: " + player.Name;
        }

        public static string GetStartingPlayer(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^starting_player: \w+$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<string>() != null);
            return Regex.Match(msg.Body, @"^starting_player: (\w+)$", RegexOptions.Multiline).Groups[1].Value;
        }

        public static string ConstructWinner(Player player) {
            Contract.Requires(player != null);
            return "winning_player: " + player.Name;
        }

        public static string GetWinner(PirateMessage msg) {
            Contract.Requires(msg != null && Regex.IsMatch(msg.Body, @"^winning_player: \w+$", RegexOptions.Multiline));
            Contract.Ensures(Contract.Result<string>() != null);
            return Regex.Match(msg.Body, @"^winning_player: (\w+)$", RegexOptions.Multiline).Groups[1].Value;
        }

        public static string ConstructPlayerTrick(Round round, Player player) {
            Contract.Requires(round != null && player != null && round.PlayerTricks.ContainsKey(player));
            return "player_tricks: " + player.Name + ";" + round.PlayerTricks[player].Count;
        }

        public static IList<string> ConstructPlayerTricks(Round round) {
            Contract.Requires(round != null);
            Contract.Ensures(Contract.Result<IList<string>>() != null);
            return round.PlayerTricks.Select(kvp => ConstructPlayerTrick(round, kvp.Key)).ToList();
        }

        public static Dictionary<string, int> GetPlayerTricks(PirateMessage msg) {
            Contract.Requires(msg != null);
            Contract.Ensures(Contract.Result<Dictionary<string, int>>() != null);
            var res = new Dictionary<string, int>();
            foreach (Match m in Regex.Matches(msg.Body, @"^player_tricks: (\w+);([0-9]+)$", RegexOptions.Multiline)) {
                res[m.Groups[1].Value] = int.Parse(m.Groups[2].Value);
            }
            return res;
        }

        public static string ConstructPlayerScore(Player player, int score) {
            Contract.Requires(player != null);
            return "player_score: " + player.Name + ";" + score;
        }

        public static IList<string> ContstructPlayerScores(Dictionary<Player, int> scores) {
            Contract.Requires(scores != null);
            Contract.Ensures(Contract.Result<IList<string>>() != null);
            return scores.Select(kvp => ConstructPlayerScore(kvp.Key, kvp.Value)).ToList();
        }

        public static Dictionary<string, int> GetPlayerScores(PirateMessage msg) {
            Contract.Requires(msg != null);
            Contract.Ensures(Contract.Result<Dictionary<string, int>>() != null);
            var res = new Dictionary<string, int>();
            foreach (Match m in Regex.Matches(msg.Body, @"^player_score: (\w+);(-?[0-9]+)$", RegexOptions.Multiline)) {
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