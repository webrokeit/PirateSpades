namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Text.RegularExpressions;

    using PirateSpades.GameLogic;

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

        public static string ConstructBody(params string[] inputs) {
            Contract.Requires(inputs != null);
            return string.Join("\n", inputs);
        }

        public static string ConstructPlayerBet(Player p) {
            return "player_bet: " + p.Name + ";" + p.Bet;
        }

        public static Dictionary<string, int> GetPlayerBets(PirateMessage msg) {
            var res = new Dictionary<string, int>();
            foreach(Match m in Regex.Matches(msg.Body, @"^player_bet: (\w+);([0-9])$", RegexOptions.Multiline)) {
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

        /// <summary>Player Information</summary>
        Pnfo,

        /// <summary>Players In Game</summary>
        Pigm, 

        /// <summary>Player Accept</summary>
        Pacp,

        /// <summary>Transfer Card</summary>
        Xcrd,

        /// <summary>Play Card</summary>
        Pcrd,

        /// <summary>Player Bet</summary>
        Pbet,

        /// <summary>Set Amount of Tricks</summary>
        Satk
    }
}