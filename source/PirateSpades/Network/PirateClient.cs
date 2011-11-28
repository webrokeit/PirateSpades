using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.Network {
    using System.Net.Sockets;

    public class PirateClient : GameLogic.Player {
        public Socket Socket { get; set; }
        public byte[] ReceiveBuffer = new byte[PirateMsg.BufferSize];
        public byte[] SendBuffer = new byte[PirateMsg.BufferSize];

        public PirateClient (Socket socket) {
            this.Socket = socket;
        }
    }
}
