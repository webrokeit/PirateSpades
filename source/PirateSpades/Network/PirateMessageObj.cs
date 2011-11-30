using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.Network {
    using System.Diagnostics.Contracts;
    using System.Net.Sockets;

    public class PirateMessageObj {
        public byte[] Buffer { get; set; }
        public PirateClient Client { get; set; }

        public PirateMessageObj(PirateClient client) {
            Contract.Requires(client != null && client.BufferSize > 0);
            this.Client = client;
            this.Buffer = new byte[client.BufferSize];
        }
    }
}
