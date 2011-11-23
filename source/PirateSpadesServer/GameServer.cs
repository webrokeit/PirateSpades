using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpadesServer {
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    internal class GameServer {
        private TcpListener Listener { get; set; }
        private int Port { get; set; }

        private List<Socket> Players { get; set; }

        private IAsyncResult asyncAcceptResult;

        public GameServer(int port) {
            this.Port = port;

            this.Players = new List<Socket>();
            this.Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), this.Port);
        }

        public void Start() {
            this.Listener.Start();
            this.WaitForSocket();
        }

        public void Stop() {
            this.Listener.Stop();
        }

        private void WaitForSocket() {
            this.Listener.BeginAcceptSocket(this.SocketConnected, Listener);
        }

        private void SocketConnected(IAsyncResult ar) {
            // lol
        }
    }
}
