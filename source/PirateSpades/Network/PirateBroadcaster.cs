// <copyright file="PirateBroadcaster.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      Used for notifying searching clients that we're hosting a game.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Sockets;
    using System.Timers;

    public class PirateBroadcaster {
        private Socket Socket { get; set; }
        private IPEndPoint EndPoint { get; set; }
        public byte[] Message { get; private set; }
        public int Port { get; private set; }
        public double Interval {
            get {
                Contract.Ensures(Contract.Result<double>() >= 0);
                return this.Timer.Interval;
            }
            set {
                Contract.Requires(value >= 0);
                this.Timer.Interval = value;
            }
        }
        private Timer Timer { get; set; }

        public delegate void BroadcastEventDelegate(PirateBroadcaster broadcaster);
        public event BroadcastEventDelegate BroadcastInitiated;
        public event BroadcastEventDelegate BroadcastExecuted;

        public PirateBroadcaster(byte[] message, int port, double interval) {
            Contract.Requires(message != null && port >= 0 && port <= 65536 && interval >= 0);
            this.Message = message;
            this.Port = port;
            this.Timer = new Timer(interval);
            this.Timer.Elapsed += Trigger;
        }

        public void Start() {
            Contract.Requires(!Timer.Enabled);
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            this.EndPoint = new IPEndPoint(IPAddress.Broadcast, Port);

            this.Broadcast();
            Timer.Start();
        }

        public void Stop() {
            Contract.Requires(Timer.Enabled);
            Timer.Stop();
            this.Socket.Close();
        }

        private void Broadcast() {
            try {
                if(BroadcastInitiated != null) BroadcastInitiated(this);
                this.Socket.SendTo(Message, this.EndPoint);
                if(BroadcastExecuted != null) BroadcastExecuted(this);
            } catch(Exception ex) {
                Console.WriteLine(ex);
            }
        }

        private void Trigger(object sender, ElapsedEventArgs e) {
            this.Broadcast();
        }
    }
}
