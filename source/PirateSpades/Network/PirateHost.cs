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

    using PirateSpades.GameLogicV2;
    using PirateSpades.Misc;

    public class PirateHost {
        private TcpListener Listener { get; set; }
        private int Port { get; set; }

        public bool Started { get; private set; }
        public bool AcceptNewConnections { get; private set; }

        private int BufferSize { get; set; }

        public bool DebugMode { get; set; }

        public Game Game { get; private set; }

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

        private struct SendObject {
            public readonly PirateClient PirateClient;
            public readonly PirateMessage PirateMessage;

            public SendObject(PirateClient pclient, PirateMessage msg) {
                this.PirateClient = pclient;
                this.PirateMessage = msg;
            }
        }

        public PirateHost(int port) {
            this.Port = port;
            this.BufferSize = 4096;

            this.Clients = new OrderedDictionary<Socket, PirateClient>();
            this.Players = new Dictionary<string, Socket>();
            this.Listener = new TcpListener(new IPEndPoint(IPAddress.Any, this.Port));
            this.Game = new Game();
        }

        public void Start() {
            Contract.Requires(!Started);
            Contract.Ensures(Started);
            this.Game = new Game();
            this.Listener.Start();
            this.WaitForSocket();
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
            var tmpClients = new List<PirateClient>(Clients.Values);
            foreach(var pclient in tmpClients) {
                this.SocketDisconnect(pclient);
            }
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

                    //Console.WriteLine("[" + read + "] " + mobj.Buffer);

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
                    buffer, 0, buffer.Length, SocketFlags.None, MessageSent, new SendObject(pclient, msg));
            } catch(SocketException ex) {
                if(!IgnoreSocketErrors.Contains(ex.SocketErrorCode)) Console.WriteLine("SocketException:" + ex);
                this.SocketDisconnect(pclient);
            }catch(Exception ex){
                Console.WriteLine(ex);
            }
        }

        private void MessageSent(IAsyncResult ar) {
            Contract.Requires(ar != null && ar.AsyncState is SendObject);
            var sendObj = (SendObject)ar.AsyncState;
            int sent = sendObj.PirateClient.Socket.EndSend(ar);
            var msg = sendObj.PirateMessage;
            // TODO: Log that the message has been sent?
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
                }
            }
        }

        public void AddClient(PirateClient pclient) {
            Contract.Requires(pclient != null);
            lock (Clients) {
                Clients[pclient.Socket] = pclient;
            }

            lock (Players) {
                if (!string.IsNullOrEmpty(pclient.Name)) {
                    Players[pclient.Name] = pclient.Socket;
                }
            }
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
        }

        public PirateClient PlayerFromSocket(Socket socket) {
            Contract.Requires(socket != null);
            lock (Clients) {
                return this.Clients.ContainsKey(socket) ? this.Clients[socket] : null;
            }
        }

        public PirateClient PlayerFromString(string s) {
            Contract.Requires(s != null);
            if(this.Players.ContainsKey(s)) {
                if(this.Clients.ContainsKey(this.Players[s])) {
                    return this.Clients[this.Players[s]];
                }
            }
            return null;
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

            lock (Clients) {
                this.Clients[pclient.Socket].SetName(name);
            }

            Console.WriteLine("Set name for " + pclient.Socket.RemoteEndPoint + " to " + name);
        }

        public IEnumerable<PirateClient> GetPlayers() {
            return this.Clients.Values;
        }
    }
}
