namespace PirateSpades.Network {
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;

    public class PirateMessage {
        public static int BufferSize = 4096;
        public PirateMessageHead Head { get; set; }
        public string Body { get; set; }

        public PirateMessage(string head, string body) : this(GetHead(head), body) {}

        public PirateMessage(PirateMessageHead head, string body) {
            this.Head = head;
            this.Body = body;
        }

        public static PirateMessage GetMessage(byte[] buffer, int len) {
            Contract.Requires(len >= 4);
            string head = Encoding.UTF8.GetString(buffer, 0, 4);
            string body = Encoding.UTF8.GetString(buffer, 4, len - 4);

            return new PirateMessage(head, body);
        }

        private static PirateMessageHead GetHead(string head) {
            PirateMessageHead pmh;
            return Enum.TryParse(head, true, out pmh) ? pmh : PirateMessageHead.Fail;
        }
    }

    public enum PirateMessageHead {
        /// <summary>Failure</summary>
        Fail, // Failure

        /// <summary>Transfer Card</summary>
        Xcrd, // Transfer Card

        /// <summary>Play Card</summary>
        Pcrd // Play Card
    }
}