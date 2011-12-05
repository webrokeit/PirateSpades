using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PirateSpades.Network;

namespace AndreasTest {
    using System.Net;
    using System.Text.RegularExpressions;

    using PirateSpades.GameLogicV2;
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
                        host.StartGame();
                        break;

                    case "exit":
                    case "e":
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

            var game = new Game();
            var pc = new PirateClient("", ip, 4939);
            pc.SetGame(game);
            pc.NameRequested += OnNameRequest;
            pc.Disconnected += OnDisconnect;
            pc.BetRequested += OnBetRequest;
            pc.CardRequested += OnCardRequest;
            Console.WriteLine("Initiating...");
            pc.InitConnection();

            while(pc.Socket.Connected) {}
            return;
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
            Console.Title = "Player: " + playerName;
            pclient.SetName(playerName);
        }

        private static void OnDisconnect(PirateClient pclient) {
            Console.WriteLine("Server disconnected!");
        }

        private static void OnBetRequest(PirateClient pclient) {
            /*pclient.SetBet(CollectionFnc.PickRandom(0, pclient.Hand.Count));
            return;*/
            var bet = string.Empty;
            var pbet = 0;
            while(string.IsNullOrEmpty(bet)) {
                Console.Write("Input bet (0 - " + pclient.Game.Round.Cards + "): ");
                bet = Console.ReadLine();
                if(bet == null || !Regex.IsMatch(bet, "^[0-9]+$")) {
                    Console.WriteLine("Invalid bet specified, try again...");
                    bet = string.Empty;
                }else {
                    pbet = int.Parse(bet);
                    if(pbet > pclient.Game.Round.Cards) {
                        pbet = pclient.Game.Round.Cards;
                    }
                }
            }
            pclient.SetBet(pbet);
        }

        private static void OnCardRequest(PirateClient pclient) {
            /*pclient.PlayCard(pclient.GetPlayableCard());
            return;*/
            var cardIndex = string.Empty;
            Card card = null;
            while(string.IsNullOrEmpty(cardIndex)) {
                Console.WriteLine("Your hand:");
                for(var i = 0; i < pclient.Hand.Count; i++) {
                    Console.WriteLine("\t" + "[" + i + "] " + pclient.Hand[i].Suit + ";" + pclient.Hand[i].Value);
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
