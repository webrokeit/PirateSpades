using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PirateSpades.Network;

namespace AndreasTest {
    using System.Net;
    using System.Text.RegularExpressions;

    using PirateSpades.GameLogicV2;
    using PirateSpades.Func;

    class Program {

        static void Main(string[] args) {
            try {
                Console.Write("Choose function (host/player): ");
                var function = Console.ReadLine();
                if(function != null) {

                    switch (function.ToLower()) {
                        case "host":
                            Host();
                            break;
                        case "player":
                            Player();
                            break;
                        default:
                            Console.WriteLine("No valid function specified!");
                            break;
                    }
                }


                /*while(!host.PlayersReady) {
                    //Console.WriteLine("Waiting for players");
                }

                List<Player> l = new List<Player> { pc, pc2 };

                Game g = new Game();
                g.AddPlayer(pc);
                g.AddPlayer(pc2);

                Table t = Table.GetTable();

                Round r = new Round(l, 5, 6);

                r.Start();

                r.CollectBet(pc, 2);
                r.CollectBet(pc2, 3);
                
                t.Start(g);

                while(pc.NumberOfCards > 0) {
                    Player first = t.PlayerTurn == pc ? pc : pc2;
                    Player second = first == pc ? pc2 : pc;

                    for(var i = 0; i < first.NumberOfCards; i++) {
                        if (first.Playable(first.Hand(i))) {
                            first.PlayCard(first.Hand(i));
                            break;
                        }
                    }

                    for(var i = 0; i < second.NumberOfCards; i++) {
                        if (second.Playable(second.Hand(i))) {
                            second.PlayCard(second.Hand(i));
                            break;
                        }
                    }
                }

                g.ReceiveStats(r);
                t.Stop();

                Console.WriteLine("Andreas Points = " + g.Points(pc));
                Console.WriteLine("Morten Points = " + g.Points(pc2));*/

            } catch(Exception ex) {
                Console.WriteLine(ex);
            } finally {
                Console.Write("Program finished, press any key to exit...");
                Console.ReadKey(true);
            }
        }

        private static void Host() {
            Console.WriteLine("Creating host");
            var host = new PirateHost(4939) { DebugMode = true };
            host.Start();
            Console.WriteLine("Host started");
            while (host.Started) {
                var cmd = Console.ReadLine();
                switch(cmd) {
                    case "start":

                        break;

                    case "exit":
                        host.Stop();
                        break;
                }
            }
        }

        private static void Player() {
            Console.Write("IP to use (empty to scan): ");
            var strIp = Console.ReadLine();
            IPAddress ip = null;
            if(!string.IsNullOrEmpty(strIp)) {
                if(!IPAddress.TryParse(strIp, out ip)) {
                    Console.WriteLine("Invalid IP specified!");
                    return;
                }

                if(!PirateScanner.CheckIp(ip, 4939, 50)) {
                    Console.WriteLine("No game is hosted at the specified ip!");
                    return;
                }
            }else {
                Console.WriteLine("Scanning for IP...");
                var d = DateTime.Now;
                ip = new PirateScanner().ScanForIp(4939);
                Console.WriteLine("Scan took " + (DateTime.Now - d).TotalMilliseconds + " milliseconds");
                if (ip == null) {
                    Console.WriteLine("No IP found... Make sure there's a host!");
                    return;
                }
                Console.WriteLine("IP Found: " + ip);
            }
            Console.WriteLine();

            var pc = new PirateClient("", ip, 4939);
            pc.NameRequested += OnNameRequest;
            pc.Disconnected += OnDisconnect;
            Console.WriteLine("Initiating...");
            pc.InitConnection();

            while(pc.Socket.Connected) {}
        }

        private static void OnNameRequest(PirateClient pclient) {
            var playerName = string.Empty;
            while (String.IsNullOrEmpty(playerName)) {
                Console.Write("Select a name [a-zA-Z0-9_] (3 - 20 chars): ");
                playerName = Console.ReadLine();
                if (playerName == null || !Regex.IsMatch(playerName, @"^\w{3,20}$")) {
                    Console.WriteLine("Invalid name specified, try again...");
                    playerName = string.Empty;
                }
            }

            pclient.SetName(playerName);
        }

        private static void OnDisconnect(PirateClient pclient) {
            Console.WriteLine("Server disconnected!");
        }
    }
}
