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
            Contract.Requires(buffer != null && readLen > 4);

            var messages = new List<PirateMessage>();
            var data = Encoding.UTF8.GetString(buffer, 0, readLen);
            if(data.Length > 4) {
                int start = 0;
                while (start < data.Length) {
                    int len = int.Parse(data.Substring(start, 4));
                    if(len >= 4) {
                        string head = data.Substring(start + 4, 4);
                        string body = data.Substring(start + 8, len - 4);

                        messages.Add(new PirateMessage(head, body));
                    }
                    start += len + 4;
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

        public PirateError GetError(string s) {
            PirateError err;
            return Enum.TryParse(s, true, out err) ? err : PirateError.Unknown;
        }

        public static string ConstructBody(params string[] inputs) {
            Contract.Requires(inputs != null);
            return string.Join("\n", inputs);
        }

        public static string ConstructPlayerBet(Player p) {
            Contract.Requires(p != null);
            return "player_bet: " + p.Name + ";" + p.Bet;
        }

        public static Dictionary<string, int> GetPlayerBets(PirateMessage msg) {
            Contract.Requires(msg != null);
            var res = new Dictionary<string, int>();
            foreach(Match m in Regex.Matches(msg.Body, @"^player_bet: (\w+);([0-9])$", RegexOptions.Multiline)) {
                res[m.Groups[1].Value] = int.Parse(m.Groups[2].Value);
            }
            return res;
        }

        public static string ConstructStartingPlayer(Player p) {
            Contract.Requires(p != null);
            return "starting_player: " + p.Name;
        }

        public static string GetStarrtingPlayer(PirateMessage msg) {
            Contract.Requires(msg != null);
            var m = Regex.Match(msg.Body, @"^starting_player: (\w+)$", RegexOptions.Multiline);
            return m.Success ? m.Groups[1].Value : null;
        }
    }

    public enum PirateMessageHead {
        /// <summary>Failure</summary>
        Fail, // Failure

        /// <summary>Error</summary>
        Erro,

        /// <summary>Knock Knock (For scanning)</summary>
        Knck,

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

        /// <summary>Transfer Card</summary>
        Xcrd,

        /// <summary>Play Card</summary>
        Pcrd,

        /// <summary>Player Bet</summary>
        Pbet,

        /// <summary>Set Amount of Tricks</summary>
        Satk
    }

    public enum PirateError {
        /// <summary>Unknown error. Also used if error send could not be identified.</summary>
        Unknown,

        /// <summary>You're already connected.</summary>
        AlreadyConnected,

        /// <summary>No new connection allowed.</summary>
        NoNewConnections,

        /// <summary>The name the user wanted to use was already taken.</summary>
        NameAlreadyTaken
    }
}