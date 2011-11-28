namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    public class PirateHost {
        private TcpListener Listener { get; set; }
        private int Port { get; set; }

        private int BufferSize { get; set; }

        private Dictionary<Socket, PirateClient> Players { get; set; }

        private IAsyncResult asyncAcceptResult;

        public PirateHost(int port) {
            this.Port = port;
            this.BufferSize = 4096;

            this.Players = new Dictionary<Socket, PirateClient>();
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
            this.Listener.BeginAcceptSocket(this.SocketConnected, null);
        }

        private void SocketConnected(IAsyncResult ar) {
            var client = this.Listener.EndAcceptSocket(ar);
            if(!Players.ContainsKey(client)) {
                var pclient = new PirateClient(client);
                Players.Add(client, pclient);
                pclient.Socket.BeginReceive(
                    pclient.ReceiveBuffer,
                    0,
                    pclient.ReceiveBuffer.Length,
                    SocketFlags.None,
                    this.SocketMessageReceived,
                    pclient);
            }else {
                // TODO: Notify client that they're already conncected.
            }
        }

        private void SocketMessageReceived(IAsyncResult ar) {
            var pclient = (PirateClient)ar.AsyncState;
            var read = pclient.Socket.EndReceive(ar);

            if(read >= 4) {
                var msg = PirateMsg.GetMessage(pclient.ReceiveBuffer, read);
            }
            

        }
    }
}
