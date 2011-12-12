using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PirateSpades.Network;

namespace AndreasTest {
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;

    using PirateSpades.GameLogic;
    using PirateSpades.Misc;

    class Program {

        static void Main(string[] args) {
            try {
                Console.Write("Choose function (host/player): ");
                var function = Console.ReadLine();
                if(function != null) {

                    switch (function.ToLower()) {
                        case "host":
                        case "h":
                            Host();
                            break;
                        case "player":
                        case "p":
                            Player();
                            break;
                        default:
                            Console.WriteLine("No valid function specified!");
                            break;
                    }
                }
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
            Console.Title = "Host";
            while (host.Started) {
                var cmd = Console.ReadLine();
                switch(cmd) {
                    case "start":
                    case "s":
                        if (!host.Game.Started) {
                            if (host.Game.Players.Count >= 2) {
                                host.StartGame();
                                host.Game.GameFinished += GameFinished;
                            } else {
                                Console.WriteLine("Not enough players to start the game!");
                            }
                        }else {
                            Console.WriteLine("Game already started!");
                        }
                        break;

                    case "exit":
                    case "e":
                        host.Stop();
                        break;
                }
            }
        }

        private static void GameFinished(Game game) {
            var sb = new StringBuilder();

            sb.AppendLine("Testing GetRoundScore");
            for(var i = 1; i <= game.RoundsPossible; i++) {
                var s = game.GetRoundScore(i);
                var t = game.GetRoundScoreTotal(i);
                sb.AppendLine("Round #" + i);
                foreach(var kvp in s) {
                    sb.AppendLine("\t" + kvp.Key.Name + ": " + kvp.Value);
                }
                sb.AppendLine("\tTotals:");
                foreach(var kvp in t) {
                    sb.AppendLine("\t\t" + kvp.Key.Name + ": " + kvp.Value);
                }
            }
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("Testing GetTotalScores");
            var st = game.GetTotalScores();
            foreach(var kvp in st) {
                sb.AppendLine(kvp.Key.Name + ": " + kvp.Value);
            }
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("Testing GetScoreTable");
            var sct = game.GetScoreTable();
            sb.Append("Round");
            foreach(var p in sct[1].Keys) {
                sb.Append("\t" + p.Name);
            }
            sb.AppendLine();
            foreach(var kvp in sct) {
                sb.Append(kvp.Key.ToString());
                foreach(var score in kvp.Value.Values) {
                    sb.Append("\t" + score);
                }
                sb.AppendLine();
            }


            var fs = new FileStream("score test.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var buffer = Encoding.Default.GetBytes(sb.ToString());
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();
        }

        private static void Player() {
            Console.Write("IP to use (empty to scan): ");
            var strIp = Console.ReadLine();
            IPAddress ip = null;
            if(!string.IsNullOrEmpty(strIp)) {
                if(!IPAddress.TryParse(strIp, out ip)) {
                    Console.WriteLine("Invalid IP specified!");
                    Player();
                    return;
                }

                if(!PirateScanner.CheckIp(ip, 4939, 5000)) {
                    Console.WriteLine("No game is hosted at the specified ip!");
                    Player();
                    return;
                }
            }else {
                Console.WriteLine("Scanning for IPs...");
                /*var d = DateTime.Now;
                ip = new PirateScanner().ScanForIp(4939);
                Console.WriteLine("Scan took " + (DateTime.Now - d).TotalMilliseconds + " milliseconds");
                if (ip == null) {
                    Console.WriteLine("No IP found... Make sure there's a host!");
                    return;
                }
                Console.WriteLine("IP Found: " + ip);*/

                var games = new PirateScanner().ScanForGames(4939, 10000);
                if(games.Count > 0) {
                    for(var i = 0; i < games.Count; i++) {
                        Console.WriteLine("\t[" + i + "] " + games[i].Ip + " \"" + games[i].GameName + "\" (" + games[i].Players + "/" + games[i].MaxPlayers + ")");
                    }
                    Console.Write("Select IP (index): ");
                    var ipIndex = Console.ReadLine();
                    if(ipIndex == null || !Regex.IsMatch(ipIndex, "^[0-9]+$")) {
                        Console.WriteLine("Invalid index specified...");
                        Player();
                        return;
                    } else {
                        int index = int.Parse(ipIndex);
                        if (index >= games.Count) {
                            Console.WriteLine("Invalid index specified...");
                            Player();
                            return;
                        }
                        ip = games[index].Ip;
                    }
                }else {
                    Console.WriteLine("No IP found... Make sure there's a host!");
                    Player();
                    return;
                }
            }
            Console.WriteLine();

            var game = new Game();
            var pc = new PirateClient("", ip, 4939);
            pc.SetGame(game);
            pc.NameRequested += OnNameRequest;
            pc.Disconnected += OnDisconnect;
            pc.BetRequested += OnBetRequest;
            pc.CardRequested += OnCardRequest;
            Console.WriteLine("Initiating...");
            pc.InitConnection();

            while(pc.Socket.Connected) {
                Thread.Sleep(10);
            }
            return;
        }

        private static void OnNameRequest(PirateClient pclient) {
            var playerName = string.Empty;
            while (String.IsNullOrEmpty(playerName)) {
                Console.Write("Select a name [a-zA-Z0-9] (3 - 12 chars): ");
                playerName = Console.ReadLine();
                if(playerName == null || !Regex.IsMatch(playerName, @"^[a-zA-Z0-9]{3,12}$")) {
                    Console.WriteLine("Invalid name specified, try again...");
                    playerName = string.Empty;
                }
            }
            Console.Title = "Player: " + playerName;
            pclient.SetName(playerName);
        }

        private static void OnDisconnect(PirateClient pclient) {
            Console.WriteLine("Server disconnected!");
        }

        private static void OnBetRequest(PirateClient pclient) {
            pclient.SetBet(CollectionFnc.PickRandom(0, pclient.Hand.Count));
            return;
            var bet = string.Empty;
            var pbet = 0;
            Console.WriteLine("Your hand:");
            for (var i = 0; i < pclient.Hand.Count; i++) {
                Console.WriteLine("\t" + "[" + i + "] " + pclient.Hand[i].ToShortString());
            }
            while (string.IsNullOrEmpty(bet)) {
                Console.Write("Input bet (0 - " + pclient.Game.Round.Cards + "): ");
                bet = Console.ReadLine();
                if (bet == null || !Regex.IsMatch(bet, "^[0-9]+$")) {
                    Console.WriteLine("Invalid bet specified, try again...");
                    bet = string.Empty;
                } else {
                    pbet = int.Parse(bet);
                    if (pbet > pclient.Game.Round.Cards) {
                        pbet = pclient.Game.Round.Cards;
                    }
                }
            }
            pclient.SetBet(pbet);
        }

        private static void OnCardRequest(PirateClient pclient) {
            pclient.PlayCard(pclient.GetPlayableCard());
            return;
            var cardIndex = string.Empty;
            Card card = null;
            while(string.IsNullOrEmpty(cardIndex)) {
                Console.WriteLine("Your hand:");
                for(var i = 0; i < pclient.Hand.Count; i++) {
                    Console.WriteLine("\t" + "[" + i + "] " + pclient.Hand[i].ToShortString());
                }
                Console.Write("Pick card to play (index): ");
                cardIndex = Console.ReadLine();
                if (cardIndex == null || !Regex.IsMatch(cardIndex, "^[0-9]+$")) {
                    Console.WriteLine("Invalid index specified, try again...");
                    cardIndex = string.Empty;
                } else {
                    int index = int.Parse(cardIndex);
                    if(index >= pclient.Hand.Count) {
                        Console.WriteLine("Invalid index specified, try again...");
                        cardIndex = string.Empty;
                    }else {
                        card = pclient.Hand[index];
                        if(!pclient.CardPlayable(card)) {
                            Console.WriteLine("Card not playable, you must play cards of the same suit when possible!");
                            cardIndex = string.Empty;
                        }
                    }
                }
            }
            pclient.PlayCard(card);
        }
    }
}
