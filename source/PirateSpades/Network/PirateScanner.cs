// <copyright file="PirateScanner.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A port scanner searching for PirateSpades games.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A port scanner searching for PirateSpades games.
    /// </summary>
    public class PirateScanner {
        /// <summary>
        /// Whether or not a check is running.
        /// </summary>
        public bool CheckRunning { get; private set; }

        /// <summary>
        /// Delegate to be used for events involving listening for broadcasts.
        /// </summary>
        /// <param name="sock">The socket.</param>
        /// <param name="ep">The endpoint.</param>
        /// <returns>A list of GameInfo objects.</returns>
        private delegate IList<GameInfo> WaitForBroadcastDelegate(Socket sock, EndPoint ep); 

        /// <summary>
        /// Delegate to be used for events involving finding GameInfo.
        /// </summary>
        /// <param name="gameInfo">GameInfo object.</param>
        public delegate void GameFoundDelegate(GameInfo gameInfo);

        /// <summary>
        /// Fires when a game has been found.
        /// </summary>
        public event GameFoundDelegate GameFound;

        /// <summary>
        /// GameInfo class, used for storing information about games found.
        /// </summary>
        public class GameInfo {
            /// <summary>
            /// The IP found.
            /// </summary>
            public IPAddress Ip { get; private set; }

            /// <summary>
            /// The game name found.
            /// </summary>
            public string GameName { get; private set; }

            /// <summary>
            /// Amount of players in game.
            /// </summary>
            public int Players { get; private set; }

            /// <summary>
            /// Max amount of players in game.
            /// </summary>
            public int MaxPlayers { get; private set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="ip">The Ip.</param>
            /// <param name="gameName">The game name.</param>
            /// <param name="players">Amount of players.</param>
            /// <param name="maxPlayers">Max amount of players.</param>
            public GameInfo(IPAddress ip, string gameName, int players, int maxPlayers) {
                Contract.Requires(ip != null && gameName != null && players >= 0 && maxPlayers >= players);
                this.Ip = ip;
                this.GameName = gameName;
                this.Players = players;
                this.MaxPlayers = maxPlayers;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PirateScanner() {
            CheckRunning = false;
        }

        /// <summary>
        /// Scan for games.
        /// </summary>
        /// <param name="port">Port to listen on.</param>
        /// <param name="timeout">Timeout for the scan.</param>
        /// <returns>A list of GameInfo objects containing information about the games.</returns>
        public IList<GameInfo> ScanForGames(int port, int timeout) {
            Contract.Requires(!CheckRunning && port >= 0 && port <= 65535 && timeout >= 0);
            Contract.Ensures(!CheckRunning);
            return ScanForGames(port, timeout, 0);
        }

        /// <summary>
        /// Scan for games.
        /// </summary>
        /// <param name="port">Port to listen on.</param>
        /// <param name="timeout">Timeout for the scan.</param>
        /// <param name="amount">The amount of games to find. 0 to wait for timeout.</param>
        /// <returns>A list of GameInfo objects containing information about the games.</returns>
        public IList<GameInfo> ScanForGames(int port, int timeout, int amount) {
            Contract.Requires(!CheckRunning && port >= 0 && port <= 65535 && timeout >= 0);
            Contract.Ensures(!CheckRunning);
            CheckRunning = true;
            var games = new Dictionary<string, GameInfo>();
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var iep = new IPEndPoint(IPAddress.Any, port);
            var start = DateTime.Now;

            try {
                sock.Bind(iep);

                while((DateTime.Now - start).TotalMilliseconds < timeout) {
                    var ep = (EndPoint)iep;

                    var del = new WaitForBroadcastDelegate(this.WaitForBroadcast);
                    var ar = del.BeginInvoke(sock, ep, null, null);
                    var maxWait = timeout - (int)(DateTime.Now - start).TotalMilliseconds;
                    if(maxWait < 1)
                        break;
                    ar.AsyncWaitHandle.WaitOne(maxWait);
                    if(ar.IsCompleted) {
                        foreach (var gameInfo in del.EndInvoke(ar).Where(gameInfo => !games.ContainsKey(gameInfo.Ip.ToString()))) {
                            if (this.GameFound != null) this.GameFound(gameInfo);
                            games.Add(gameInfo.Ip.ToString(), gameInfo);
                        }
                        if(amount > 0 && games.Count >= amount) break;
                    }
                }

                sock.Close();
            } catch(Exception ex) {
                Console.WriteLine(ex);
            }

            CheckRunning = false;
            return games.Values.ToList();
        }

        /// <summary>
        /// Wait for a broadcast message to be received.
        /// </summary>
        /// <param name="sock">Socket to listen on.</param>
        /// <param name="ep">Endpoint.</param>
        /// <returns>List of GameInfo objects found.</returns>
        private IList<GameInfo> WaitForBroadcast(Socket sock, EndPoint ep) {
            var res = new List<GameInfo>();
            try {
                var buffer = new byte[sock.ReceiveBufferSize];
                var read = sock.ReceiveFrom(buffer, ref ep);
                if(read > 4) {
                    res.AddRange(
                        PirateMessage.GetMessages(buffer, read).Where(msg => msg.Head == PirateMessageHead.Bcst).Select(
                            GetGameInfo));
                }
            } catch(Exception ex) {
                if(!(ex is ObjectDisposedException) && !(ex is SocketException)) {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        /// <summary>
        /// Get a GameInfo object from a PirateMessage.
        /// </summary>
        /// <param name="msg">The PirateMessage.</param>
        /// <returns>The GameInfo object from the message.</returns>
        private GameInfo GetGameInfo(PirateMessage msg) {
            Contract.Requires(msg != null);
            Contract.Ensures(Contract.Result<GameInfo>() != null);

            var ip = PirateMessage.GetHostIp(msg);
            var gameName = PirateMessage.GetGameName(msg);
            var players = PirateMessage.GetPlayersInGame(msg);
            var maxPlayers = PirateMessage.GetMaxPlayersInGame(msg);

            return new GameInfo(ip, gameName, players, maxPlayers);
        }

        /// <summary>
        /// Check if a game is hosted on the specified ip and port.
        /// </summary>
        /// <param name="ip">Ip to check on.</param>
        /// <param name="port">Port to check on.</param>
        /// <param name="timeout">Max wait.</param>
        /// <returns></returns>
        public static bool CheckIp(IPAddress ip, int port, int timeout) {
            Contract.Requires(ip != null && port >= 0 && port <= 65535 && timeout >= 0 && IsValidIp(ip));
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var res = false;
            try {
                var ar = socket.BeginConnect(ip, port, null, null);
                ar.AsyncWaitHandle.WaitOne(timeout);
                if(socket.Connected) {
                    res = PirateClientCommands.KnockKnock(socket);
                }
            } catch (SocketException ex) {
                Console.WriteLine("Exception: " + ex);
            } finally {
                socket.Close();
            }
            
            return res;
        }

        /// <summary>
        /// Get a local IPv4 address.
        /// </summary>
        /// <returns>A local IPv4 address.</returns>
        [Pure]
        public static IPAddress GetLocalIpV4() {
            return GetLocalIpsV4().First();
        }

        /// <summary>
        /// Get the local IPv4 addresses.
        /// </summary>
        /// <returns>The local IPv4 addresses.</returns>
        [Pure]
        public static IEnumerable<IPAddress> GetLocalIpsV4() {
            return Dns.GetHostAddresses(Dns.GetHostName()).Where(IsValidIp).Distinct();
        }

        /// <summary>
        /// Checks if the specified Ip is valid or not.
        /// </summary>
        /// <param name="ip">Ip to check</param>
        /// <returns>True if the ip is valid, false if not.</returns>
        [Pure]
        public static bool IsValidIp(string ip) {
            Contract.Requires(ip != null);
            const string Pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            return Regex.IsMatch(ip, Pattern);
        }

        /// <summary>
        /// Checks if the specified Ip is valid or not.
        /// </summary>
        /// <param name="ip">Ip to check</param>
        /// <returns>True if the ip is valid, false if not.</returns>
        [Pure]
        public static bool IsValidIp(IPAddress ip) {
            Contract.Requires(ip != null);
            return IsValidIp(ip.ToString());
        }

        /// <summary>
        /// Get an Ip from a string.
        /// </summary>
        /// <param name="ip">The ip string.</param>
        /// <returns>The IP from the string.</returns>
        [Pure]
        public static IPAddress GetIp(string ip) {
            Contract.Requires(ip != null && IsValidIp(ip));
            Contract.Ensures(Contract.Result<IPAddress>() != null);
            const string Pattern = @"^((?:[1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(?:\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3})$";
            return IPAddress.Parse(Regex.Match(ip, Pattern).Groups[1].Value);
        }
    }
}
