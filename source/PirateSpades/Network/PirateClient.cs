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
    using System.Net;
    using System.Diagnostics.Contracts;
    using System.Net.Sockets;

    using PirateSpades.GameLogic;

    /// <summary>
    /// A network client for the PirateSpades game.
    /// </summary>
    public class PirateClient : Player {
        /// <summary>
        /// The socket to use for communicating.
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// The buffer to use for messages.
        /// </summary>
        public int BufferSize { get; private set; }

        /// <summary>
        /// Whether or not debug mode is in use.
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        /// Whether or not the client is a virtual player.
        /// </summary>
        public bool VirtualPlayer { get; private set; }

        /// <summary>
        /// Whether or not the client is dead.
        /// </summary>
        public bool IsDead { get; private set; }

        /// <summary>
        /// Delegate to be used for events involving PirateClient.
        /// </summary>
        /// <param name="pclient">The PirateClient.</param>
        public delegate void PirateClientDelegate(PirateClient pclient);

        /// <summary>
        /// Fires when the client has been disconnected.
        /// </summary>
        public event PirateClientDelegate Disconnected;

        /// <summary>
        /// Fires when a name request was received.
        /// </summary>
        public event PirateClientDelegate NameRequested;

        /// <summary>
        /// Fires when a bet request was received.
        /// </summary>
        public event PirateClientDelegate BetRequested;

        /// <summary>
        /// Fires when card request was received.
        /// </summary>
        public event PirateClientDelegate CardRequested;

        /// <summary>
        /// A collection of socket errors to ignore.
        /// </summary>
        private static readonly HashSet<SocketError> IgnoreSocketErrors = new HashSet<SocketError>() { SocketError.ConnectionReset };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket">The name of the player.</param>
        public PirateClient (Socket socket) : base("") {
            Contract.Requires(socket != null);
            this.Socket = socket;
            this.VirtualPlayer = true;
            this.Init();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="ip">The ip to connect to.</param>
        /// <param name="port">The port to use.</param>
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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="ip">The ip to connect to.</param>
        /// <param name="port">The port to use.</param>
        public PirateClient (string name, string ip, int port) : this(name, IPAddress.Parse(ip), port) {
            Contract.Requires(name != null && ip != null && port > 0 && port <= 65535 && PirateScanner.IsValidIp(ip));
        }

        /// <summary>
        /// Initialize the client.
        /// </summary>
        private void Init() {
            IsDead = false;
            BufferSize = PirateMessage.BufferSize;
            if (!VirtualPlayer) {
                this.CardPlayed += this.OnCardPlayed;
                this.CardDealt += this.OnCardDealt;
                this.BetSet += this.OnBetSet;
            }
        }

        /// <summary>
        /// Initialize the connection to the host.
        /// </summary>
        public void InitConnection() {
            PirateClientCommands.InitConnection(this);
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        public void Disconnect() {
            if(this.Socket.Connected) {
                this.Socket.Close();
                IsDead = true;
            }
            if(Disconnected != null) {
                Disconnected(this);
            }
        }

        /// <summary>
        /// When a card has been played.
        /// </summary>
        /// <param name="card">The card played.</param>
        private void OnCardPlayed(Card card) {
            PirateClientCommands.PlayCard(this, card);
        }

        /// <summary>
        /// When a card has been dealt.
        /// </summary>
        /// <param name="p">Receiving player.</param>
        /// <param name="c">Card dealt.</param>
        private void OnCardDealt(Player p, Card c) {
            PirateClientCommands.DealCard(this, p, c);
        }

        /// <summary>
        /// When a bet has been set.
        /// </summary>
        /// <param name="bet">The bet.</param>
        private void OnBetSet(int bet) {
            PirateClientCommands.SetBet(this, bet);
        }

        /// <summary>
        /// When the name has been set.
        /// </summary>
        /// <param name="name">The name.</param>
        public void SetName(string name) {
            Contract.Requires(name != null);
            this.Name = name;
            if(!VirtualPlayer) {
                PirateClientCommands.SendPlayerInfo(this);
            }
        }

        /// <summary>
        /// Receive socket messages asynchronously.
        /// </summary>
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

        /// <summary>
        /// When a message was received through the socket.
        /// </summary>
        /// <param name="ar">AsyncResult containing information about the asynchronous operation.</param>
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

        /// <summary>
        /// Send socket message asynchronously.
        /// </summary>
        /// <param name="msg">The message to send.</param>
        public void SendMessage(PirateMessage msg) {
            Contract.Requires(msg != null);
            var buffer = msg.GetBytes();
            Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, MessageSent, msg);
        }

        /// <summary>
        /// When a message has been sent.
        /// </summary>
        /// <param name="ar">AsyncResult containing information about the asynchronous operation.</param>
        private void MessageSent(IAsyncResult ar) {
            Contract.Requires(ar != null && ar.AsyncState is PirateMessage);
            var sent = Socket.EndSend(ar);
            var msg = (PirateMessage)ar.AsyncState;
            // TODO: Log that the message has been sent?
        }

        /// <summary>
        /// Handle the incoming message.
        /// </summary>
        /// <param name="msg">The message.</param>
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

        /// <summary>
        /// Request bet.
        /// </summary>
        public void RequestBet() {
            if(BetRequested != null) BetRequested(this);
        }

        /// <summary>
        /// Request card.
        /// </summary>
        public void RequestCard() {
            if (CardRequested != null) CardRequested(this);
        }

        /// <summary>
        /// Name not available.
        /// </summary>
        public void NameNotAvailable() {
            if (NameRequested != null) NameRequested(this);
        }

        public override string ToString() {
            Contract.Ensures(Contract.Result<string>() != null);
            return PirateMessage.ConstructPlayerName(Name);
        } 
    }
}
