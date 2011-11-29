namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;

    using PirateSpades.GameLogic;

    public class PirateClient : GameLogic.Player {
        public readonly Socket Socket;
        public byte[] ReceiveBuffer = new byte[PirateMessage.BufferSize];
        public byte[] SendBuffer = new byte[PirateMessage.BufferSize];

        public PirateClient (Socket socket) {
            Contract.Requires(socket != null);
            this.Socket = socket;
            this.CardPlayed += OnCardPlayed;
        }

        public PirateClient (string address, int port) {
            Contract.Requires(address != null && port > 0 && port <= 65535);
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Socket.Connect(address, port);
        }

        private void OnCardPlayed(Card c) {
            Contract.Requires(c != null);
            var msg = new PirateMessage(PirateMessageHead.Pcrd, this.ToString() + c.ToString());
            this.SendMessage(msg);
        }

        private void SendMessage(PirateMessage msg) {
            Contract.Requires(msg != null);
            var buffer = msg.GetBytes();
            Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, MessageSent, msg);
        }

        private void MessageSent(IAsyncResult ar) {
            Contract.Requires(ar != null && ar.AsyncState is PirateMessage);
            var sent = Socket.EndSend(ar);
            var msg = (PirateMessage)ar.AsyncState;
            // TODO: Log that the message has been sent?
        }

        public override string ToString() {
            Contract.Requires(Socket.LocalEndPoint != null);
            return "player:{" /*+ Socket.LocalEndPoint.ToString() + ","*/ + "name" + "}";
        }

        public static string NameFromString(string s) {
            var m = Regex.Match(s, @"player:\{([a-zA-Z]+)\}");
            return m.Success ? m.Groups[1].Value : null;
        }
    }
}
