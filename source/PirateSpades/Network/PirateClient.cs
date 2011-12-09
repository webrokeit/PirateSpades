// <copyright file="PirateClient.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A network client for the PirateSpades game.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Diagnostics.Contracts;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;

    using PirateSpades.GameLogicV2;

    public class PirateClient : Player {
        public readonly Socket Socket;
        public int BufferSize { get; private set; }

        public bool DebugMode { get; set; }

        public bool VirtualPlayer { get; private set; }
        public bool IsDead { get; private set; }

        public delegate void PirateClientDelegate(PirateClient pclient);
        public event PirateClientDelegate Disconnected;
        public event PirateClientDelegate NameRequested;
        public event PirateClientDelegate BetRequested;
        public event PirateClientDelegate CardRequested;

        private static readonly HashSet<SocketError> IgnoreSocketErrors = new HashSet<SocketError>() { SocketError.ConnectionReset };

        public PirateClient (Socket socket) : base("") {
            Contract.Requires(socket != null);
            this.Socket = socket;
            this.VirtualPlayer = true;
            this.Init();
        }

        public PirateClient(string name, IPAddress ip, int port) : base(name) {
            Contract.Requires(name != null && ip != null && port > 0 && port <= 65535 && PirateScanner.IsValidIp(ip));
            this.VirtualPlayer = false;
            this.Init();
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                this.Socket.Connect(ip, port);
                if(this.Socket.Connected) {
                    this.SocketMessageReceive();
                }else {
                    IsDead = true;
                }
            } catch(SocketException ex) {
                Console.WriteLine("Failed to connect: " + ex);
                IsDead = true;
            }
        }

        public PirateClient (string name, string ip, int port) : this(name, IPAddress.Parse(ip), port) {
            Contract.Requires(name != null && ip != null && port > 0 && port <= 65535 && PirateScanner.IsValidIp(ip));
        }

        private void Init() {
            IsDead = false;
            BufferSize = PirateMessage.BufferSize;
            if (!VirtualPlayer) {
                this.CardPlayed += this.OnCardPlayed;
                this.CardDealt += this.OnCardDealt;
                this.BetSet += this.OnBetSet;
            }
        }

        public void InitConnection() {
            PirateClientCommands.InitConnection(this);
        }

        private void Disconnect() {
            if(this.Socket.Connected) {
                this.Socket.Close();
                IsDead = true;
            }
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

        private void OnBetSet(int bet) {
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
                case PirateMessageHead.Erro:
                    PirateClientCommands.ErrorMessage(this, msg);
                    break;
                case PirateMessageHead.Init:
                    PirateClientCommands.VerifyConnection(this, msg);
                    break;
                case PirateMessageHead.Pnfo:
                    if (string.IsNullOrEmpty(Name)) {
                        if (NameRequested != null) NameRequested(this);
                    } else {
                        PirateClientCommands.SendPlayerInfo(this);
                    }
                    break;
                case PirateMessageHead.Xcrd:
                    PirateClientCommands.GetCard(this, msg);
                    break;
                case PirateMessageHead.Pcrd:
                    PirateClientCommands.GetPlayedCard(this, msg);
                    break;
                case PirateMessageHead.Pigm:
                    PirateClientCommands.GetPlayersInGame(this, msg);
                    break;
                case PirateMessageHead.Gstr:
                    PirateClientCommands.GameStarted(this, msg);
                    break;
                case PirateMessageHead.Gfin:
                    PirateClientCommands.GameFinished(this, msg);
                    break;
                case PirateMessageHead.Trdn:
                    PirateClientCommands.NewPile(this, msg);
                    break;
                case PirateMessageHead.Nrnd:
                    PirateClientCommands.NewRound(this, msg);
                    break;
                case PirateMessageHead.Bgrn:
                    PirateClientCommands.BeginRound(this, msg);
                    break;
                case PirateMessageHead.Frnd:
                    PirateClientCommands.FinishRound(this, msg);
                    break;
                case PirateMessageHead.Breq:
                    this.RequestBet();
                    break;
                case PirateMessageHead.Creq:
                    this.RequestCard();
                    break;
            }
        }

        public void RequestBet() {
            if(BetRequested != null) BetRequested(this);
        }

        public void RequestCard() {
            if (CardRequested != null) CardRequested(this);
        }

        public void NameNotAvailable() {
            if (NameRequested != null) NameRequested(this);
        }

        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);
            return PirateMessage.ConstructPlayerName(Name);
        } 
    }
}
