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

    public class PirateHost {
        private TcpListener Listener { get; set; }
        public IPAddress Ip { get; private set; }
        public int Port { get; private set; }

        public bool Started { get; private set; }
        public bool AcceptNewConnections { get; private set; }

        public bool DebugMode { get; set; }

        public Game Game { get; private set; }
        public string GameName { get; private set; }
        public int MaxPlayers { get; private set; }
        public PirateBroadcaster Broadcaster { get; private set; }

        public int PlayerCount {
            get {
                return Players.Count;
            }
        }

        public bool PlayersReady {
            get {
                return this.Clients.Count == this.Players.Count && this.Clients.Count > 0 && this.Clients.Values.All(pclient => !String.IsNullOrEmpty(pclient.Name));
            }
        }

        private static readonly HashSet<SocketError> IgnoreSocketErrors = new HashSet<SocketError>() { SocketError.ConnectionReset };

        private OrderedDictionary<Socket, PirateClient> Clients { get; set; }
        private Dictionary<string, Socket> Players { get; set; }

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

        public void Start() {
            Contract.Requires(!Started);
            Contract.Ensures(Started);

            this.Start(PirateScanner.GetLocalIpV4().ToString().Replace(".", ""), Game.MaxPlayersInGame);
        }

        public void Start(string gameName) {
            Contract.Requires(!Started && !string.IsNullOrEmpty(gameName));
            Contract.Ensures(Started);

            this.Start(gameName, Game.MaxPlayersInGame);
        }

        public void Start(int maxPlayers) {
            Contract.Requires(!Started && maxPlayers >= Game.MinPlayersInGame && maxPlayers <= Game.MaxPlayersInGame);
            Contract.Ensures(Started);

            this.Start(PirateScanner.GetLocalIpV4().ToString().Replace(".", ""), maxPlayers);
        }

        public void Start(string gameName, int maxPlayers){
            Contract.Requires(!Started && !string.IsNullOrEmpty(gameName) && maxPlayers >= Game.MinPlayersInGame && maxPlayers <= Game.MaxPlayersInGame);
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

        public void StopAccepting() {
            AcceptNewConnections = false;
        }

        public void Stop() {
            Contract.Requires(Started);
            Contract.Ensures(!Started);
            Started = false;
            Broadcaster.Stop();
            var tmpClients = new List<PirateClient>(Clients.Values);
            foreach(var pclient in tmpClients) {
                this.SocketDisconnect(pclient);
            }
            this.Clients.Clear();
            this.Players.Clear();
            this.Listener.Stop();
        }

        private void WaitForSocket() {
            this.Listener.BeginAcceptSocket(SocketConnected, this);
        }

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

        public void OnBroadcastExecuted(PirateBroadcaster broadcaster) {
            if(DebugMode) Console.WriteLine("Broadcasted IP");
        }

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

        public PirateClient PlayerFromSocket(Socket socket) {
            Contract.Requires(socket != null && this.Clients.ContainsKey(socket));
            Contract.Ensures(Contract.Result<PirateClient>() != null);
            lock (Clients) {
                return this.Clients[socket];
            }
        }

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

        public PirateClient PlayerFromIndex(int i) {
            Contract.Requires(i >= 0 && i < Clients.Count);
            Contract.Ensures(Contract.Result<PirateClient>() != null);
            lock(Clients) {
                return Clients[i];
            }
        }

        public bool ContainsPlayer(PirateClient pclient) {
            Contract.Requires(pclient != null);
            return ContainsPlayer(pclient.Socket);
        }

        public bool ContainsPlayer(Socket socket) {
            Contract.Requires(socket != null);
            lock(Clients) {
                return Clients.ContainsKey(socket);
            }
        }

        public bool ContainsPlayer(string playerName) {
            Contract.Requires(playerName != null);
            lock(Players) {
                return Players.ContainsKey(playerName);
            }
        }

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

        public IEnumerable<PirateClient> GetPlayers() {
            return this.Clients.Values;
        }

        public void StartGame() {
            Contract.Requires(Game != null && !Game.Started);
            PirateHostCommands.StartGame(this);
        }

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

        private void UpdateBroadcastInfo() {
            var msg = new PirateMessage(PirateMessageHead.Bcst, PirateMessage.ConstructHostInfo(this));
            this.Broadcaster.Message = msg.GetBytes();
        }

        private void RoundStarted(Game game) {
            PirateHostCommands.NewRound(this);
        }

        private void RoundBegun(Game game) {
            PirateHostCommands.RequestCard(this, Clients[game.Round.CurrentPlayer]);
        }

        private void RoundNewPile(Game game) {
            PirateHostCommands.NewPile(this);
        }

        private void RoundFinished(Game game) {
            PirateHostCommands.RoundFinished(this);
        }

        private void GameFinished(Game game) {
            PirateHostCommands.GameFinished(this);
        }

        public static bool IsValidGameName(string gameName) {
            Contract.Requires(!string.IsNullOrEmpty(gameName));
            return Regex.IsMatch(gameName, @"^[a-zA-Z0-9]{1,12}$");
        }
    }
}
