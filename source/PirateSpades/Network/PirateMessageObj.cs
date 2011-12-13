// <copyright file="PirateMessageObj.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      Message object used for reading asynchonously.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Message object used for reading asynchonously.
    /// </summary>
    public class PirateMessageObj {
        /// <summary>
        /// Byte buffer.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// The client.
        /// </summary>
        public PirateClient Client { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">The client.</param>
        public PirateMessageObj(PirateClient client) {
            Contract.Requires(client != null && client.BufferSize > 0);
            this.Client = client;
            this.Buffer = new byte[client.BufferSize];
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="msg">The message to get the byte buffer from.</param>
        public PirateMessageObj(PirateClient client, PirateMessage msg) {
            Contract.Requires(client != null && msg != null);
            this.Client = client;
            this.Buffer = msg.GetBytes();
        }
    }
}
