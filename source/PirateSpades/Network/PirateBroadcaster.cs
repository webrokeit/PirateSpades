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

    /// <summary>
    /// Used for notifying searching clients that we're hosting a game.
    /// </summary>
    public class PirateBroadcaster {
        /// <summary>
        /// The socket to use to broadcast.
        /// </summary>
        private Socket Socket { get; set; }

        /// <summary>
        /// The endpoint to use when broadcasting.
        /// </summary>
        private IPEndPoint EndPoint { get; set; }

        /// <summary>
        /// The bytes of the message to broadcast.
        /// </summary>
        public byte[] Message { get; set; }

        /// <summary>
        /// The port to use when broadcasting.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// The interval to broadcast in.
        /// </summary>
        public double Interval {
            get {
                Contract.Ensures(Contract.Result<double>() > 0.00);
                return this.Timer.Interval;
            }
            set {
                Contract.Requires(value > 0.00 && value < int.MaxValue);
                this.Timer.Interval = value;
            }
        }

        /// <summary>
        /// Whether or not the broadcaster is enabled!
        /// </summary>
        public bool Enabled {
            get {
                return Timer.Enabled;
            }
        }

        /// <summary>
        /// The timer to execute the broadcast method.
        /// </summary>
        private Timer Timer { get; set; }

        /// <summary>
        /// Delegate to be used for events involving the broadcaster.
        /// </summary>
        /// <param name="broadcaster">The broadcaster.</param>
        public delegate void BroadcastEventDelegate(PirateBroadcaster broadcaster);

        /// <summary>
        /// Fires right before a broadcast is about to be send.
        /// </summary>
        public event BroadcastEventDelegate BroadcastInitiated;

        /// <summary>
        /// Fires right after a broadcast has been sent.
        /// </summary>
        public event BroadcastEventDelegate BroadcastExecuted;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">The port to broadcast on.</param>
        /// <param name="interval">The interval to broadcast in.</param>
        public PirateBroadcaster(int port, double interval) {
            Contract.Requires(port >= 0 && port <= 65536 && interval > 0.00 && interval < int.MaxValue);
            this.Port = port;
            this.Timer = new Timer(interval);
            this.Timer.Elapsed += Trigger;
            this.Message = new byte[] { };
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="port">The port to broadcast on.</param>
        /// <param name="interval">The interval to broadcast in.</param>
        public PirateBroadcaster(byte[] message, int port, double interval) : this(port, interval) {
            Contract.Requires(message != null && port >= 0 && port <= 65536 && interval > 0.00 && interval < int.MaxValue);
            this.Message = message;
        }

        /// <summary>
        /// Start broadcasting.
        /// </summary>
        public void Start() {
            Contract.Requires(!Timer.Enabled);
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            this.EndPoint = new IPEndPoint(IPAddress.Broadcast, Port);

            this.Broadcast();
            Timer.Start();
        }

        /// <summary>
        /// Stop broadcasting.
        /// </summary>
        public void Stop() {
            Contract.Requires(Timer.Enabled);
            Timer.Stop();
            this.Socket.Close();
        }

        /// <summary>
        /// Do the broadcast.
        /// </summary>
        private void Broadcast() {
            try {
                if(BroadcastInitiated != null) BroadcastInitiated(this);
                this.Socket.SendTo(Message, this.EndPoint);
                if(BroadcastExecuted != null) BroadcastExecuted(this);
            } catch(Exception ex) {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Event being fired by the timer.
        /// </summary>
        /// <param name="sender">The timer.</param>
        /// <param name="e">Event args.</param>
        private void Trigger(object sender, ElapsedEventArgs e) {
            this.Broadcast();
        }
    }
}
