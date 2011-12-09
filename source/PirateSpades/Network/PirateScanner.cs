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

    using PirateSpades.Misc;

    public class PirateScanner {
        public bool CheckRunning { get; private set; }

        private delegate IList<GameInfo> WaitForBroadcastDelegate(Socket sock, EndPoint ep); 
        public delegate void GameFoundDelegate(GameInfo gameInfo);
        public event GameFoundDelegate GameFound;

        public class GameInfo {
            public IPAddress Ip { get; private set; }
            public string GameName { get; private set; }
            public int Players { get; private set; }
            public int MaxPlayers { get; private set; }

            public GameInfo(IPAddress ip, string gameName, int players, int maxPlayers) {
                this.Ip = ip;
                this.GameName = gameName;
                this.Players = players;
                this.MaxPlayers = maxPlayers;
            }

            public override int GetHashCode() {
                return Ip.GetHashCode();
            }

            public override bool Equals(object obj) {
                return Ip.Equals(obj);
            }
        }

        public PirateScanner() {
            CheckRunning = false;
        }

        public IList<GameInfo> ScanForGames(int port, int timeout) {
            Contract.Requires(!CheckRunning && port >= 0 && port <= 65535 && timeout >= 0);
            Contract.Ensures(!CheckRunning);
            return ScanForGames(port, timeout, 0);
        }

        public IList<GameInfo> ScanForGames(int port, int timeout, int amount) {
            Contract.Requires(!CheckRunning && port >= 0 && port <= 65535 && timeout >= 0);
            Contract.Ensures(!CheckRunning);
            CheckRunning = true;
            var games = new HashSet<GameInfo>();
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
                        foreach(var gameInfo in del.EndInvoke(ar)) {
                            if (this.GameFound != null) this.GameFound(gameInfo);
                            games.Add(gameInfo);
                        }
                        if(amount > 0 && games.Count >= amount) break;
                    }
                }

                sock.Close();
            } catch(Exception ex) {
                Console.WriteLine(ex);
            }

            CheckRunning = false;
            return games.ToList();
        }

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

        private GameInfo GetGameInfo(PirateMessage msg) {
            Contract.Requires(msg != null);
            Contract.Ensures(Contract.Result<GameInfo>() != null);

            var ip = PirateMessage.GetHostIp(msg);
            var gameName = PirateMessage.GetGameName(msg);
            var players = PirateMessage.GetPlayersInGame(msg);
            var maxPlayers = PirateMessage.GetMaxPlayersInGame(msg);

            return new GameInfo(ip, gameName, players, maxPlayers);
        }

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

        [Pure]
        public static IPAddress GetLocalIpV4() {
            return GetLocalIpsV4().First();
        }

        [Pure]
        public static IEnumerable<IPAddress> GetLocalIpsV4() {
            return Dns.GetHostAddresses(Dns.GetHostName()).Where(IsValidIp).Distinct();
        }

        [Pure]
        public static bool IsValidIp(string ip) {
            Contract.Requires(ip != null);
            const string Pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            return Regex.IsMatch(ip, Pattern);
        }

        [Pure]
        public static bool IsValidIp(IPAddress ip) {
            Contract.Requires(ip != null);
            return IsValidIp(ip.ToString());
        }

        [Pure]
        public static IPAddress GetIp(string ip) {
            Contract.Requires(ip != null && IsValidIp(ip));
            Contract.Ensures(Contract.Result<IPAddress>() != null);
            const string Pattern = @"^((?:[1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(?:\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3})$";
            return IPAddress.Parse(Regex.Match(ip, Pattern).Groups[1].Value);
        }
    }
}
