﻿// <copyright file="PirateScanner.cs">
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
        private readonly List<IPAddress> ipsToCheck = new List<IPAddress>();

        public int IpCount { get; private set; }
        public int IpsChecked { get; private set; }
        public bool CheckRunning { get; private set; }

        public double PercentageDone {
            get {
                if (this.IpCount == 0) {
                    return 0.00;
                } else {
                    return ((double)this.IpsChecked / this.IpCount) * 100.0;
                }
            }
        }

        public PirateScanner() {
            this.IpCount = 0;
            this.IpsChecked = 0;
        }

        public IList<IPAddress> ScanForIps(int port, int timeout, int amount = 0) {
            Contract.Requires(port >= 0 && port <= 65535 && timeout >= 0);
            var ips = new HashSet<IPAddress>();
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
                        foreach(var ipString in del.EndInvoke(ar)) {
                            IPAddress ip = null;
                            if(IPAddress.TryParse(ipString, out ip)) {
                                ips.Add(ip);
                            }
                        }
                        if(amount > 0 && ips.Count >= amount) break;
                    }
                }

                sock.Close();
            } catch(Exception ex) {
                Console.WriteLine(ex);
            }

            return ips.ToList();
        }

        private delegate IList<string> WaitForBroadcastDelegate(Socket sock, EndPoint ep); 

        private IList<string> WaitForBroadcast(Socket sock, EndPoint ep) {
            var res = new List<string>();
            try {
                var buffer = new byte[512];
                var read = sock.ReceiveFrom(buffer, ref ep);
                if(read > 4) {
                    res.AddRange(
                        PirateMessage.GetMessages(buffer, read).Where(msg => msg.Head == PirateMessageHead.Bcst).Select(
                            msg => msg.Body));
                }
            } catch(Exception ex) {
                if(!(ex is ObjectDisposedException) && !(ex is SocketException)) {
                    Console.WriteLine(ex);
                }
            }
            return res;
        }

        public IPAddress ScanForIp(int port) {
            Contract.Requires(port >= 0 && port <= 65535 && !CheckRunning);
            Contract.Ensures(!CheckRunning);

            CheckRunning = true;
            ipsToCheck.Clear();
            var ipSubs = new HashSet<string>();
            foreach (var ipBits in GetLocalIpsV4().Select(localIp => Regex.Split(localIp.ToString(), "\\."))) {
                ipSubs.Add(string.Join(".", ipBits, 0, 3));
            }

            foreach(var ipSub in ipSubs) {
                for (var i = 0; i < 256; i++) {
                    ipsToCheck.Add(IPAddress.Parse(ipSub + "." + i));
                }
            }

            CollectionFnc.FisherYatesShuffle(ipsToCheck);
            IpCount = ipsToCheck.Count();
            IpsChecked = 0;

            while (IpsChecked < IpCount) {
                if (CheckIp(ipsToCheck[IpsChecked], port, 50)) {
                    CheckRunning = false;
                    return ipsToCheck[IpsChecked];
                }
                IpsChecked++;
            }

            CheckRunning = false;
            return null;
        }

        public static bool CheckIp(IPAddress ip, int port, int timeout) {
            Contract.Requires(ip != null && port >= 0 && port <= 65535 && timeout >= 0 && IsValidIp(ip));
            //Console.WriteLine("Checking ip: " + ip.ToString());
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
    }
}
