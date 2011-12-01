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
    using System.Text;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Remoting.Messaging;
    using System.Text.RegularExpressions;

    public class PirateScanner {
        private readonly List<IPAddress> ipsToCheck = new List<IPAddress>();

        public int IpCount { get; private set; }
        public int IpsChecked { get; private set; }

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

        public IPAddress ScanForIp(int port) {
            Contract.Requires(port >= 0 && port <= 65535);

            ipsToCheck.Clear();
            var ipSubs = new HashSet<string>();
            foreach (var ipBits in GetLocalIPs().Select(localIp => Regex.Split(localIp.ToString(), "\\."))) {
                ipSubs.Add(string.Join(".", ipBits, 0, 3));
            }

            foreach(var ipSub in ipSubs) {
                for (var i = 0; i < 256; i++) {
                    ipsToCheck.Add(IPAddress.Parse(ipSub + "." + i));
                }
            }

            IpCount = ipsToCheck.Count();
            IpsChecked = 0;

            while (IpsChecked < IpCount) {
                if (CheckIp(ipsToCheck[IpsChecked], port, 50)) {
                    return ipsToCheck[IpsChecked];
                }
                IpsChecked++;
            }

            return this.ipsToCheck.FirstOrDefault(ip => CheckIp(ip, port, 50));
        }

        public static bool CheckIp(IPAddress ip, int port, int timeout) {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var res = false;
            try {
                var ar = socket.BeginConnect(ip, port, null, null);
                ar.AsyncWaitHandle.WaitOne(timeout);
                res = socket.Connected;
            } catch (SocketException ex) {
                Console.WriteLine("Exception: " + ex);
            } finally {
                socket.Close();
            }
            
            return res;
        }

        private static IEnumerable<IPAddress> GetLocalIPs() {
            return Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => Regex.IsMatch(ip.ToString(), @"[0-9]{0,3}\.[0-9]{0,3}\.[0-9]{0,3}\.[0-9]{0,3}")).Distinct();
        }
    }
}
