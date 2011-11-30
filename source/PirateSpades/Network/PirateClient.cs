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
        public int BufferSize { get; private set; }

        public PirateClient (Socket socket) : base("") {
            Contract.Requires(socket != null);
            this.Socket = socket;
            this.Init();
        }

        public PirateClient (string name, string address, int port) : base(name) {
            Contract.Requires(address != null && port > 0 && port <= 65535);
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Socket.Connect(address, port);
            this.Init();
            this.SocketMessageReceive();
        }

        private void Init() {
            BufferSize = PirateMessage.BufferSize;
            this.CardPlayed += OnCardPlayed;
            this.CardDealt += this.OnCardDealt;
        }

        private void OnCardPlayed(Card card) {
            PirateClientCommands.PlayCard(this, card);
        }

        private void OnCardDealt(Player p, Card c) {
            PirateClientCommands.DealCard(this, p, c);
        }

        private void OnBetSet(Player p, int bet) {
            
        }

        public void SetName(string name) {
            this.Name = name;
        }

        private void SocketMessageReceive() {
            var mobj = new PirateMessageObj(this);
            Socket.BeginReceive(
                    mobj.Buffer,
                    0,
                    mobj.Buffer.Length,
                    SocketFlags.None,
                    SocketMessageReceived,
                    mobj
            );
        }

        private void SocketMessageReceived(IAsyncResult ar) {
            Contract.Requires(ar != null && ar.AsyncState is PirateMessageObj);
            var mobj = (PirateMessageObj)ar.AsyncState;
            var read = Socket.EndReceive(ar);

            if(read >= 4) {
                var msg = PirateMessage.GetMessage(mobj.Buffer, read);
                HandleMessage(msg);
            }

            if(Socket.Connected) {
                SocketMessageReceive();
            }
        }

        public void SendMessage(PirateMessage msg) {
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

        private void HandleMessage(PirateMessage msg) {
            switch(msg.Head) {
                case PirateMessageHead.Pnfo:
                    PirateClientCommands.SendPlayerInfo(this);
                    break;
                case PirateMessageHead.Xcrd:
                    PirateClientCommands.GetCard(this, msg);
            }
        }

        public override string ToString() {
            Contract.Requires(Socket.LocalEndPoint != null);
            return PirateMessage.ConstructBody("player_name: " + Name, "player_ip: 0.0.0.0");
        }

        public static string NameToString(string name) {
            return "player_name: " + name;
        }

        public static string NameFromString(string s) {
            var m = Regex.Match(s, @"^player_name: ([a-zA-Z]+)$", RegexOptions.Multiline);
            return m.Success ? m.Groups[1].Value : null;
        }
    }
}
