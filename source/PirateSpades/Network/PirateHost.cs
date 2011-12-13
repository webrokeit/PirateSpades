// <copyright file="PirateHost.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A game host for the PirateSpades game.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;

    using PirateSpades.GameLogic;
    using PirateSpades.Misc;

    /// <summary>
    /// A game host for the PirateSpades game.
    /// </summary>
    public class PirateHost {
        /// <summary>
        /// The listener.
        /// </summary>
        private TcpListener Listener { get; set; }

        /// <summary>
        /// The host ip.
        /// </summary>
        public IPAddress Ip { get; private set; }

        /// <summary>
        /// The port to communicate on.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Whether or not the host has been started.
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        /// Whether or not the host is accepting new connections.
        /// </summary>
        public bool AcceptNewConnections { get; private set; }

        /// <summary>
        /// Whether or not debug mode is enabled.
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        /// The game of the host.
        /// </summary>
        public Game Game { get; private set; }

        /// <summary>
        /// The game name of the host.
        /// </summary>
        public string GameName { get; private set; }

        /// <summary>
        /// The amount of max players.
        /// </summary>
        public int MaxPlayers { get; private set; }

        /// <summary>
        /// The broadcaster.
        /// </summary>
        public PirateBroadcaster Broadcaster { get; private set; }

        /// <summary>
        /// The amount of players.
        /// </summary>
        public int PlayerCount {
            get {
                return Players.Count;
            }
        }

        /// <summary>
        /// Whether or not all clients active have set a name.
        /// </summary>
        public bool PlayersReady {
            get {
                return this.Clients.Count == this.Players.Count && this.Clients.Count > 0 && this.Clients.Values.All(pclient => !String.IsNullOrEmpty(pclient.Name));
            }
        }

        /// <summary>
        /// Collection of socket errors to ignore.
        /// </summary>
        private static readonly HashSet<SocketError> IgnoreSocketErrors = new HashSet<SocketError>() { SocketError.ConnectionReset };

        /// <summary>
        /// Clients and their sockets in the order they joined.
        /// </summary>
        private OrderedDictionary<Socket, PirateClient> Clients { get; set; }

        /// <summary>
        /// Player names their corresponding socket.
        /// </summary>
        private Dictionary<string, Socket> Players { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">The port to communicate over.</param>
        public PirateHost(int port) {
            Contract.Requires(port > 0 && port <= 65535);
            this.Ip = PirateScanner.GetLocalIpV4();
            this.Port = port;

            this.Clients = new OrderedDictionary<Socket, PirateClient>();
            this.Players = new Dictionary<string, Socket>();
            this.Listener = new TcpListener(new IPEndPoint(IPAddress.Any, this.Port));

            var msg = new PirateMessage(PirateMessageHead.Bcst, Ip.ToString());
            this.Broadcaster = new PirateBroadcaster(this.Port, 6250);
        }

        /// <summary>
        /// Start the host.
        /// Game name will be the hosting IP with dots removed.
        /// </summary>
        public void Start() {
            Contract.Requires(!Started);
            Contract.Ensures(Started);

            this.Start(PirateScanner.GetLocalIpV4().ToString().Replace(".", ""), Game.MaxPlayersInGame);
        }

        /// <summary>
        /// Start the host and use the specified game name.
        /// </summary>
        /// <param name="gameName">The game name to use.</param>
        public void Start(string gameName) {
            Contract.Requires(!Started && !string.IsNullOrEmpty(gameName));
            Contract.Ensures(Started);

            this.Start(gameName, Game.MaxPlayersInGame);
        }

        /// <summary>
        /// Start the host and use the specified max players.
        /// </summary>
        /// <param name="maxPlayers">Max amount of players in game.</param>
        public void Start(int maxPlayers) {
            Contract.Requires(!Started && maxPlayers >= Game.MinPlayersInGame && maxPlayers <= Game.MaxPlayersInGame);
            Contract.Ensures(Started);

            this.Start(PirateScanner.GetLocalIpV4().ToString().Replace(".", ""), maxPlayers);
        }

        /// <summary>
        /// Start the host and use the specified game name and the max amount of players.
        /// </summary>
        /// <param name="gameName">The game name to use.</param>
        /// <param name="maxPlayers">Max amount of players in game.</param>
        public void Start(string gameName, int maxPlayers){
            Contract.Requires(!Started && IsValidGameName(gameName) && maxPlayers >= Game.MinPlayersInGame && maxPlayers <= Game.MaxPlayersInGame);
            Contract.Ensures(Started);

            this.GameName = gameName;
            this.MaxPlayers = maxPlayers;
            this.NewGame();
            this.Listener.Start();
            this.WaitForSocket();

            this.UpdateBroadcastInfo();
            Broadcaster.BroadcastExecuted += this.OnBroadcastExecuted;
            Broadcaster.Start();
            
            Started = true;
            AcceptNewConnections = true;
        }

        /// <summary>
        /// Stop accepting new connections.
        /// </summary>
        public void StopAccepting() {
            AcceptNewConnections = false;
            if(Broadcaster.Enabled) Broadcaster.Stop();
        }

        /// <summary>
        /// Stop the host.
        /// </summary>
        public void Stop() {
            Contract.Requires(Started);
            Contract.Ensures(!Started);
            Started = false;
            if(Broadcaster.Enabled) Broadcaster.Stop();
            var tmpClients = new List<PirateClient>(Clients.Values);
            foreach(var pclient in tmpClients) {
                this.SocketDisconnect(pclient);
            }
            this.Clients.Clear();
            this.Players.Clear();
            this.Listener.Stop();
        }

        /// <summary>
        /// Start waiting for new connections.
        /// </summary>
        private void WaitForSocket() {
            this.Listener.BeginAcceptSocket(SocketConnected, this);
        }

        /// <summary>
        /// New connection established.
        /// </summary>
        /// <param name="ar">AsyncResult containing information about the asynchronous operation.</param>
        private void SocketConnected(IAsyncResult ar) {
            Contract.Requires(ar != null && ar.AsyncState is PirateHost);
            try {
                var host = (PirateHost)ar.AsyncState;
                if (Started) {
                    var client = host.Listener.EndAcceptSocket(ar);
                    var pclient = new PirateClient(client);

                    if(AcceptNewConnections) {
                        if (DebugMode) Console.WriteLine("Client connected: " + client.RemoteEndPoint.ToString());

                        host.WaitForSocket(); // Wait for more
                        
                        if (!host.Clients.ContainsKey(pclient.Socket)) {
                            host.Clients.Add(pclient.Socket, pclient);
                            SocketMessageReceive(pclient);
                        } else {
                            PirateHostCommands.ErrorMessage(this, pclient, PirateError.AlreadyConnected);
                        }
                    } else {
                        PirateHostCommands.ErrorMessage(this, pclient, PirateError.NoNewConnections);
                    }
                }
            } catch (SocketException ex) {
                if(!IgnoreSocketErrors.Contains(ex.SocketErrorCode)) Console.WriteLine("SocketException: " + ex);
            } catch (Exception ex) {
                Console.WriteLine("Exception: " + ex);
            }
        }

        /// <summary>
        /// Disconnect a client.
        /// </summary>
        /// <param name="pclient">The client to disconnect.</param>
        private void SocketDisconnect(PirateClient pclient) {
            Contract.Requires(pclient != null);
            if (pclient.Socket != null) {
                this.RemoveClient(pclient);
                if (pclient.Socket.RemoteEndPoint != null) {
                    var clientIp = pclient.Socket.RemoteEndPoint.ToString();
                    if (DebugMode) Console.WriteLine("Client disconnected " + (!string.IsNullOrEmpty(pclient.Name) ? "[" + pclient.Name + "]" : "") + ": " + clientIp);
                }
                if (pclient.Socket.Connected) {
                    pclient.Socket.Close();
                }
                if(Started && !string.IsNullOrEmpty(pclient.Name)) {
                    PirateHostCommands.SendPlayerInfo(this);
                }
            }
        }

        /// <summary>
        /// Start receiving messages from a client.
        /// </summary>
        /// <param name="pclient">The client to receive messages from.</param>
        private void SocketMessageReceive(PirateClient pclient) {
            Contract.Requires(pclient != null);
            var mobj = new PirateMessageObj(pclient);
            pclient.Socket.BeginReceive(
                    mobj.Buffer,
                    0,
                    mobj.Buffer.Length,
                    SocketFlags.None,
                    SocketMessageReceived,
                    mobj
            );
        }

        /// <summary>
        /// Received message from client.
        /// </summary>
        /// <param name="ar">AsyncResult containing information about the asynchronous operation.</param>
        private void SocketMessageReceived(IAsyncResult ar) {
            Contract.Requires(ar != null && ar.AsyncState is PirateMessageObj);
            try {
                var mobj = (PirateMessageObj)ar.AsyncState;
                var pclient = mobj.Client;

                if (pclient.Socket.Connected) {

                    var read = pclient.Socket.EndReceive(ar);

                    if (read >= 4) {
                        foreach (var msg in PirateMessage.GetMessages(mobj.Buffer, read)) {
                            this.HandleMessage(pclient, msg);
                        }
                    } else if (read == 0) {
                        this.SocketDisconnect(pclient);
                    }

                    if(pclient.Socket.Connected) SocketMessageReceive(pclient);
                }
            } catch (SocketException ex) {
                if (!IgnoreSocketErrors.Contains(ex.SocketErrorCode)) Console.WriteLine("SocketException: " + ex);
                
                var mobj = (PirateMessageObj)ar.AsyncState;
                var pclient = mobj.Client;
                this.SocketDisconnect(pclient);
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Send message to a client.
        /// </summary>
        /// <param name="pclient">Client to send to.</param>
        /// <param name="msg">Message to send.</param>
        public void SendMessage(PirateClient pclient, PirateMessage msg) {
            Contract.Requires(pclient != null && msg != null);
            try {
                byte[] buffer = msg.GetBytes();
                pclient.Socket.BeginSend(
                    buffer, 0, buffer.Length, SocketFlags.None, MessageSent, new PirateMessageObj(pclient, msg));
            } catch(SocketException ex) {
                if(!IgnoreSocketErrors.Contains(ex.SocketErrorCode)) Console.WriteLine("SocketException:" + ex);
                this.SocketDisconnect(pclient);
            }catch(Exception ex){
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Message sent to client.
        /// </summary>
        /// <param name="ar">AsyncResult containing information about the asynchronous operation.</param>
        private void MessageSent(IAsyncResult ar) {
            Contract.Requires(ar != null && ar.AsyncState is PirateMessageObj);
            try {
                var sendObj = (PirateMessageObj)ar.AsyncState;
                if(!sendObj.Client.IsDead) {
                    var sent = sendObj.Client.Socket.EndSend(ar);
                    // TODO: Log that the message has been sent?
                }
            } catch(SocketException ex) {
                if(!IgnoreSocketErrors.Contains(ex.SocketErrorCode)) Console.WriteLine("SocketException:" + ex);
            } catch(Exception ex) {
                if(!(ex is ObjectDisposedException)) {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Handle message received.
        /// </summary>
        /// <param name="pclient">Client that send the message.</param>
        /// <param name="msg">The message.</param>
        private void HandleMessage(PirateClient pclient, PirateMessage msg) {
            Contract.Requires(pclient != null && msg != null);
            if (!Players.ContainsKey(pclient.Name)) {
                switch (msg.Head) {
                    case PirateMessageHead.Knck:
                        PirateHostCommands.KnockKnock(this, pclient);
                        break;
                    case PirateMessageHead.Init:
                        PirateHostCommands.InitConnection(this, pclient, msg);
                        break;
                    case PirateMessageHead.Verf:
                        PirateHostCommands.VerifyConnection(this, pclient, msg);
                        break;
                    case PirateMessageHead.Pnfo:
                        PirateHostCommands.SetPlayerInfo(this, pclient, msg);
                        break;
                }
            } else {
                switch (msg.Head) {
                    case PirateMessageHead.Xcrd:
                        PirateHostCommands.DealCard(this, msg);
                        break;
                    case PirateMessageHead.Pcrd:
                        PirateHostCommands.PlayCard(this, msg);
                        break;
                    case PirateMessageHead.Pbet:
                        PirateHostCommands.ReceiveBet(this, pclient, msg);
                        break;
                }
            }
        }

        /// <summary>
        /// When a broadcast has been executed.
        /// </summary>
        /// <param name="broadcaster">The broadcaster that executed.</param>
        public void OnBroadcastExecuted(PirateBroadcaster broadcaster) {
            if(DebugMode) Console.WriteLine("Broadcasted IP");
        }

        /// <summary>
        /// Remove a client.
        /// </summary>
        /// <param name="pclient">Client to be removed.</param>
        public void RemoveClient(PirateClient pclient) {
            Contract.Requires(pclient != null);
            if (!this.Clients.ContainsKey(pclient.Socket)) {
                return;
            }

            lock (Players) {
                if (!String.IsNullOrEmpty(this.Clients[pclient.Socket].Name)) {
                    if (this.Players.ContainsKey(this.Clients[pclient.Socket].Name)) {
                        this.Players.Remove(this.Clients[pclient.Socket].Name);
                    }
                }
            }

            lock (Clients) {
                this.Clients.Remove(pclient.Socket);
            }
            UpdateBroadcastInfo();
        }

        /// <summary>
        /// Get a player from a socket.
        /// </summary>
        /// <param name="socket">Socket to use as identifier.</param>
        /// <returns>The player associated with the socket.</returns>
        public PirateClient PlayerFromSocket(Socket socket) {
            Contract.Requires(socket != null && this.Clients.ContainsKey(socket));
            Contract.Ensures(Contract.Result<PirateClient>() != null);
            lock (Clients) {
                return this.Clients[socket];
            }
        }

        /// <summary>
        /// Get a player from a string.
        /// </summary>
        /// <param name="s">The string to use as identifier.</param>
        /// <returns>The player associated with the string.</returns>
        public PirateClient PlayerFromString(string s) {
            Contract.Requires(s != null && this.Players.ContainsKey(s));
            Contract.Ensures(Contract.Result<PirateClient>() != null);
            Socket socket;
            lock(Players) {
                socket = this.Players[s];
            }
            lock(Clients) {
                return this.Clients[socket];
            }
        }

        /// <summary>
        /// Get a player from an index.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The player at the specified index.</returns>
        public PirateClient PlayerFromIndex(int i) {
            Contract.Requires(i >= 0 && i < Clients.Count);
            Contract.Ensures(Contract.Result<PirateClient>() != null);
            lock(Clients) {
                return Clients[i];
            }
        }

        /// <summary>
        /// Checks whether or not the specified player is contained in the host player list.
        /// </summary>
        /// <param name="pclient">Player to check for.</param>
        /// <returns>True if the player is within, false if not.</returns>
        public bool ContainsPlayer(PirateClient pclient) {
            Contract.Requires(pclient != null);
            return ContainsPlayer(pclient.Socket);
        }

        /// <summary>
        /// Checks whether or not the specified player is contained in the host player list.
        /// </summary>
        /// <param name="socket">Socket to check for.</param>
        /// <returns>True if the player is within, false if not.</returns>
        public bool ContainsPlayer(Socket socket) {
            Contract.Requires(socket != null);
            lock(Clients) {
                return Clients.ContainsKey(socket);
            }
        }

        /// <summary>
        /// Checks whether or not the specified player is contained in the host player list.
        /// </summary>
        /// <param name="playerName">Player to check for.</param>
        /// <returns>True if the player is within, false if not.</returns>
        public bool ContainsPlayer(string playerName) {
            Contract.Requires(playerName != null);
            lock(Players) {
                return Players.ContainsKey(playerName);
            }
        }

        /// <summary>
        /// Set the name of the specified player.
        /// </summary>
        /// <param name="pclient">The player to set the name for.</param>
        /// <param name="name">The name </param>
        public void SetPlayerName(PirateClient pclient, string name) {
            Contract.Requires(pclient != null && name != null && this.Clients.ContainsKey(pclient.Socket));
            lock (Players) {
                if (!String.IsNullOrEmpty(this.Clients[pclient.Socket].Name)) {
                    if (this.Players.ContainsKey(this.Clients[pclient.Socket].Name)) {
                        this.Players.Remove(this.Clients[pclient.Socket].Name);
                    }
                }
                this.Players.Add(name, pclient.Socket);
            }

            pclient.SetName(name);

            Console.WriteLine("Set name for " + pclient.Socket.RemoteEndPoint + " to " + name);
            UpdateBroadcastInfo();
        }

        /// <summary>
        /// Get the players.
        /// </summary>
        /// <returns>An enumerable of players.</returns>
        public IEnumerable<PirateClient> GetPlayers() {
            return this.Clients.Values;
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        public void StartGame() {
            Contract.Requires(Game != null && !Game.Started);
            PirateHostCommands.StartGame(this);
        }

        /// <summary>
        /// New game.
        /// </summary>
        private void NewGame() {
            if (Game != null) {
                Game.RoundStarted -= RoundStarted;
                Game.RoundBegun -= RoundBegun;
                Game.RoundFinished -= RoundFinished;
                Game.GameFinished -= GameFinished;
                Game.RoundNewPile -= this.RoundNewPile;
            }
            Game = new Game();
            Game.RoundStarted += RoundStarted;
            Game.RoundBegun += RoundBegun;
            Game.RoundFinished += RoundFinished;
            Game.GameFinished += GameFinished;
            Game.RoundNewPile += this.RoundNewPile;
        }

        /// <summary>
        /// Update broadcast information.
        /// </summary>
        private void UpdateBroadcastInfo() {
            var msg = new PirateMessage(PirateMessageHead.Bcst, PirateMessage.ConstructHostInfo(this));
            this.Broadcaster.Message = msg.GetBytes();
        }

        /// <summary>
        /// Round started.
        /// </summary>
        /// <param name="game">The game.</param>
        private void RoundStarted(Game game) {
            PirateHostCommands.NewRound(this);
        }

        /// <summary>
        /// Round begun.
        /// </summary>
        /// <param name="game">The game.</param>
        private void RoundBegun(Game game) {
            PirateHostCommands.RequestCard(this, Clients[game.Round.CurrentPlayer]);
        }

        /// <summary>
        /// New pile has been created.
        /// </summary>
        /// <param name="game">The game.</param>
        private void RoundNewPile(Game game) {
            PirateHostCommands.NewPile(this);
        }

        /// <summary>
        /// Round finished.
        /// </summary>
        /// <param name="game">The game.</param>
        private void RoundFinished(Game game) {
            PirateHostCommands.RoundFinished(this);
        }

        /// <summary>
        /// Game finished.
        /// </summary>
        /// <param name="game">The game.</param>
        private void GameFinished(Game game) {
            PirateHostCommands.GameFinished(this);
        }

        /// <summary>
        /// Whether or not the game name specified is valid.
        /// </summary>
        /// <param name="gameName">The game name to test for.</param>
        /// <returns>True if it is valid, false if not.</returns>
        public static bool IsValidGameName(string gameName) {
            Contract.Requires(!string.IsNullOrEmpty(gameName));
            return Regex.IsMatch(gameName, @"^[a-zA-Z0-9]{1,12}$");
        }
    }
}
