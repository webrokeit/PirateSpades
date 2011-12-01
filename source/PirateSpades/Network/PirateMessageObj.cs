// <copyright file="PirateMessageObj.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      Message object used for reading asynchonously.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
