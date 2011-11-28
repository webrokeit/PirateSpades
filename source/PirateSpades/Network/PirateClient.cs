using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.Network {
    using System.Net.Sockets;

    public class PirateClient : GameLogic.Player {
        public Socket Socket { get; set; }
        public byte[] ReceiveBuffer = new byte[PirateMessage.BufferSize];
        public byte[] SendBuffer = new byte[PirateMessage.BufferSize];

        public PirateClient (Socket socket) {
            this.Socket = socket;
        }


    }
}
