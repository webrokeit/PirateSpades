namespace PirateSpades.Network {
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;

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

        public static PirateMessage GetMessage(byte[] buffer, int len) {
            Contract.Requires(buffer != null && buffer.Length >= len && len >= 4);
            var head = Encoding.UTF8.GetString(buffer, 0, 4);
            var body = Encoding.UTF8.GetString(buffer, 4, len - 4);

            return new PirateMessage(head, body);
        }

        private static PirateMessageHead GetHead(string head) {
            PirateMessageHead pmh;
            return Enum.TryParse(head, true, out pmh) ? pmh : PirateMessageHead.Fail;
        }

        public byte[] GetBytes() {
            return Encoding.UTF8.GetBytes(Head.ToString().ToUpper() + Body);
        }

        public static string ConstructBody(params string[] inputs) {
            Contract.Requires(inputs != null);
            return string.Join("\n", inputs);
        }
    }

    public enum PirateMessageHead {
        /// <summary>Failure</summary>
        Fail, // Failure

        /// <summary>Error</summary>
        Erro,

        /// <summary>Player Information</summary>
        Pnfo,

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