// <copyright file="PirateMessageObj.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      Message object used for reading asynchonously.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System.Diagnostics.Contracts;

    public class PirateMessageObj {
        public byte[] Buffer { get; private set; }
        public PirateClient Client { get; private set; }

        public PirateMessageObj(PirateClient client) {
            Contract.Requires(client != null && client.BufferSize > 0);
            this.Client = client;
            this.Buffer = new byte[client.BufferSize];
        }

        public PirateMessageObj(PirateClient client, PirateMessage msg) {
            Contract.Requires(client != null && msg != null);
            this.Client = client;
            this.Buffer = msg.GetBytes();
        }
    }
}
