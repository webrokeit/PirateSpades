namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Sockets;

    public class PirateHost {
        private TcpListener Listener { get; set; }
        private int Port { get; set; }

        public bool Started { get; private set; }

        private int BufferSize { get; set; }

        private Dictionary<Socket, PirateClient> Clients { get; set; }
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

            this.Clients = new Dictionary<Socket, PirateClient>();
            this.Players = new Dictionary<string, Socket>();
            this.Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), this.Port);
        }

        public void Start() {
            Contract.Requires(!Started);
            Contract.Ensures(Started);
            this.Listener.Start();
            this.WaitForSocket();
            Started = true;
        }

        public void Stop() {
            Contract.Requires(Started);
            Contract.Ensures(!Started);
            this.Listener.Stop();
            Started = false;
        }

        private void WaitForSocket() {
            this.Listener.BeginAcceptSocket(SocketConnected, this);
        }

        private void SocketConnected(IAsyncResult ar) {
            Contract.Requires(ar != null && ar.AsyncState is PirateHost);
            var host = (PirateHost)ar.AsyncState;
            var client = host.Listener.EndAcceptSocket(ar);
            
            host.WaitForSocket(); // Wait for more

            var pclient = new PirateClient(client);
            if(!host.Clients.ContainsKey(pclient.Socket)) {
                
                host.Clients.Add(pclient.Socket, pclient);
                SocketMessageReceive(pclient);
                PirateHostCommands.GetPlayerInfo(this, pclient);
            } else {
                const string Body = "You're already connected.";
                var msg = new PirateMessage(PirateMessageHead.Erro, Body);
                this.SendMessage(pclient, msg);
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
            var mobj = (PirateMessageObj)ar.AsyncState;
            var pclient = mobj.Client;
            var read = pclient.Socket.EndReceive(ar);

            if (read >= 4) {
                var msg = PirateMessage.GetMessage(mobj.Buffer, read);
                HandleMessage(pclient, msg);
            }

            if (pclient.Socket.Connected) {
                SocketMessageReceive(pclient);
            }
        }

        public void SendMessage(PirateClient pclient, PirateMessage msg) {
            Contract.Requires(pclient != null && msg != null);
            byte[] buffer = msg.GetBytes();
            pclient.Socket.BeginSend(
                buffer, 0, buffer.Length, SocketFlags.None, MessageSent, new SendObject(pclient, msg));
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
            switch (msg.Head) {
                case PirateMessageHead.Pnfo:
                    PirateHostCommands.SetPlayerInfo(this, pclient, msg);
                    // pclient.Id = "GENERATE IT";
                    // Send back id
                    break;
                case PirateMessageHead.Xcrd:
                    PirateHostCommands.DealCard(this, msg);
                    break;
                case PirateMessageHead.Pcrd:
                    PirateHostCommands.PlayCard(this, msg);
                    break;
            }
        }

        public PirateClient PlayerFromSocket(Socket socket) {
            Contract.Requires(socket != null);
            return this.Clients.ContainsKey(socket) ? this.Clients[socket] : null;
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
            if(!String.IsNullOrEmpty(this.Clients[pclient.Socket].Name)) {
                if(this.Players.ContainsKey(this.Clients[pclient.Socket].Name)) {
                    this.Players.Remove(this.Clients[pclient.Socket].Name);
                }
            }

            this.Clients[pclient.Socket].SetName(name);
            this.Players.Add(name, pclient.Socket);
        }

        public IEnumerable<PirateClient> GetPlayers() {
            return this.Clients.Values;
        }
    }
}
