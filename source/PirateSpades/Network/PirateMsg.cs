using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.Network {
    using System.Diagnostics.Contracts;

    public class PirateMsg {
        public static int BufferSize = 4096;
        public string Head { get; set; }
        public string Body { get; set; }

        public PirateMsg(string head, string body) {
            this.Head = head;
            this.Body = body;
        }

        public static PirateMsg GetMessage(byte[] buffer, int len) {
            Contract.Requires(len >= 4);
            string head = Encoding.UTF8.GetString(buffer, 0, 4);
            string body = Encoding.UTF8.GetString(buffer, 4, len - 4);
            return new PirateMsg(head, body);
        }
    }
}
