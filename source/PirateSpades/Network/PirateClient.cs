namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Diagnostics.Contracts;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;

    using PirateSpades.GameLogic;

    public class PirateClient : GameLogic.Player {
        public readonly Socket Socket;
        public int BufferSize { get; private set; }

        public bool VirtualPlayer { get; private set; }

        public delegate void PirateClientDelegate(PirateClient pclient);
        public event PirateClientDelegate Disconnected;
        public event PirateClientDelegate NameRequested;

        private static readonly HashSet<SocketError> IgnoreSocketErrors = new HashSet<SocketError>() { SocketError.ConnectionReset };

        public PirateClient (Socket socket) : base("") {
            Contract.Requires(socket != null);
            this.Socket = socket;
            this.Init();
            this.VirtualPlayer = true;
        }

        public PirateClient(string name, IPAddress ip, int port) : base(name) {
            Contract.Requires(name != null && ip != null && port > 0 && port <= 65535);
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Socket.Connect(ip, port);
            this.Init();
            this.SocketMessageReceive();
            this.VirtualPlayer = false;
        }

        public PirateClient (string name, string ip, int port) : this(name, IPAddress.Parse(ip), port) {
            Contract.Requires(name != null && ip != null && port > 0 && port <= 65535);
        }

        private void Init() {
            BufferSize = PirateMessage.BufferSize;
            this.CardPlayed += OnCardPlayed;
            this.CardDealt += this.OnCardDealt;
        }

        public void InitConnection() {
            PirateClientCommands.InitConnection(this);
        }

        private void Disconnect() {
            if(this.Socket.Connected) this.Socket.Close();
            if(Disconnected != null) {
                Disconnected(this);
            }
        }

        private void OnCardPlayed(Card card) {
            PirateClientCommands.PlayCard(this, card);
        }

        private void OnCardDealt(Player p, Card c) {
            PirateClientCommands.DealCard(this, p, c);
        }

        private void OnBetSet(Player p, int bet) {
            PirateClientCommands.SetBet(this, bet);
        }

        public void SetName(string name) {
            this.Name = name;
            if(!VirtualPlayer) {
                PirateClientCommands.SendPlayerInfo(this);
            }
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
            try {
                var mobj = (PirateMessageObj)ar.AsyncState;

                if (Socket.Connected) {
                    var read = Socket.EndReceive(ar);

                    if (read >= 4) {
                        foreach (var msg in PirateMessage.GetMessages(mobj.Buffer, read)) {
                            this.HandleMessage(msg);
                        }
                    } else if (read == 0) {
                        this.Disconnect();
                    }

                    if (Socket.Connected) SocketMessageReceive();
                }
            }catch(SocketException ex) {
                if(!IgnoreSocketErrors.Contains(ex.SocketErrorCode)) Console.WriteLine("SocketException: " + ex);
                this.Disconnect();
            }catch(Exception ex) {
                Console.WriteLine("Exception: " + ex);
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
                case PirateMessageHead.Init:
                    PirateClientCommands.VerifyConnection(this, msg);
                    break;
                case PirateMessageHead.Pnfo:
                    //PirateClientCommands.SendPlayerInfo(this);
                    if(NameRequested != null) {
                        NameRequested(this);
                    }
                    break;
                case PirateMessageHead.Xcrd:
                    PirateClientCommands.GetCard(this, msg);
                    break;
                case PirateMessageHead.Pigm:
                    PirateClientCommands.GetPlayersInGame(this, msg);
                    break;
            }
        }

        public override string ToString() {
            return NameToString(Name);
        }

        public static string NameToString(string name) {
            return "player_name: " + name;
        }

        public static string NameFromString(string s) {
            Contract.Requires(s != null);
            var m = Regex.Match(s, @"^player_name: ([a-zA-Z0-9_-]{3,20})$", RegexOptions.Multiline);
            return m.Success ? m.Groups[1].Value : null;
        }

        public static HashSet<string> NamesFromString(string s) {
            Contract.Requires(s != null);
            var res = new HashSet<string>();
            foreach(Match m in Regex.Matches(s, @"^player_name: ([a-zA-Z0-9_-]{3,20})$", RegexOptions.Multiline)) {
                res.Add(m.Groups[1].Value);
            }
            return res;
        } 
    }
}
