// <copyright file="PirateClientCommands.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      Various commands used by the PirateClientt to communicate with its host (PirateHost).
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System;
    using System.Linq;
    using System.Net.Sockets;
    using System.Diagnostics.Contracts;

    using PirateSpades.GameLogic;

    public class PirateClientCommands {
        public static bool KnockKnock(Socket client) {
            Contract.Requires(client != null);
            var knock = new PirateMessage(PirateMessageHead.Knck, "");
            client.Send(knock.GetBytes());
            var buffer = new byte[PirateMessage.BufferSize];
            var read = client.Receive(buffer);
            return read > 4 && PirateMessage.GetMessages(buffer, read).Any(msg => msg.Head == PirateMessageHead.Knck);
        }

        public static void ErrorMessage(PirateClient pclient, PirateMessage msg) {
            Contract.Requires(pclient != null && msg != null && msg.Head == PirateMessageHead.Erro);
            var err = PirateMessage.GetError(msg.Body);
            switch(err) {
                case PirateError.AlreadyConnected:
                    Console.WriteLine("You're already conncted!");
                    break;
                case PirateError.InvalidBet:
                    Console.WriteLine("Invalid bet specified!");
                    break;
                case PirateError.NameAlreadyTaken:
                    Console.WriteLine("Name is already taken!");
                    pclient.NameNotAvailable();
                    break;
                case PirateError.NoNewConnections:
                    Console.WriteLine("No more players can connect!");
                    break;
                case PirateError.Unknown:
                    Console.WriteLine("Unknown error happened!");
                    break;
            }
        }

        public static void InitConnection(PirateClient pclient) {
            Contract.Requires(pclient != null);
            var msg = new PirateMessage(PirateMessageHead.Init, "");
            pclient.SendMessage(msg);
        }

        public static void VerifyConnection(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Init);
            var msg = new PirateMessage(PirateMessageHead.Verf, data.Body);
            pclient.SendMessage(msg);
        }

        public static void SendPlayerInfo(PirateClient pclient) {
            Contract.Requires(pclient != null);
            var msg = new PirateMessage(PirateMessageHead.Pnfo, pclient.ToString());
            pclient.SendMessage(msg);
        }

        public static void GetPlayersInGame(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null);

            pclient.Game.ClearPlayers();
            var players = PirateMessage.GetPlayerNames(data);
            if(players.Count > 0) {
                Console.WriteLine("Current players in game:");
               foreach(var player in players) {
                   var p = pclient.Name == player ? pclient : new Player(player);
                   p.SetGame(pclient.Game);
                   pclient.Game.AddPlayer(p);
                   Console.WriteLine("\t" + player + (pclient.Name == player ? " (YOU)" : ""));
               }
            }
        }

        public static void GameStarted(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Gstr);

            var startPlayer = PirateMessage.GetStartingPlayer(data);
            if (startPlayer == null) return;

            if (!pclient.Game.Contains(startPlayer)) return;

            pclient.Game.Start(pclient.Game.PlayerIndex(startPlayer));
        }

        public static void GameFinished(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Gfin);

            var scores = PirateMessage.GetPlayerScores(data);
            var winner = PirateMessage.GetWinner(data);

            Console.WriteLine("GAME FINISHED - Scores:");
            foreach (var kvp in scores) {
                Console.WriteLine("\t" + kvp.Key + ": " + kvp.Value + (kvp.Key == winner ? " [WINNER!!!]" : ""));
            }
        }

        public static void PlayCard(PirateClient pclient, Card card) {
            Contract.Requires(pclient != null && card != null);
            var body = PirateMessage.ConstructBody(pclient.ToString(), card.ToString());
            var msg = new PirateMessage(PirateMessageHead.Pcrd, body);
            pclient.SendMessage(msg);
        }

        public static void GetPlayedCard(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Pcrd);

            var playerName = PirateMessage.GetPlayerName(data);
            if (playerName == null) return;

            var player = pclient.Game.GetPlayer(playerName);
            var card = Card.FromString(data.Body);
            
            if (pclient.Name != playerName) {
                player.GetCard(card);
                player.PlayCard(card);
            }

            Console.WriteLine(player.Name + " plays " + card.ToShortString());
        }

        public static void DealCard(PirateClient pclient, Player receiver, Card card) {
            Contract.Requires(pclient != null && receiver != null && card != null);
            var body = PirateMessage.ConstructBody(PirateMessage.ConstructPlayerName(receiver.Name), card.ToString());
            var msg = new PirateMessage(PirateMessageHead.Xcrd, body);

            if(pclient.DebugMode) Console.WriteLine(pclient.Name + ": Dealing " + card.ToShortString() + " to " + receiver.Name);
            pclient.SendMessage(msg);
        }

        public static void GetCard(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null);
            var card = Card.FromString(data.Body);
            if(card == null) return;

            if (pclient.DebugMode) Console.WriteLine(pclient.Name + ": Received " + card.ToShortString());
            if (!pclient.IsDealer) {
                pclient.GetCard(card);
            }
        }

        public static void SetBet(PirateClient pclient, int bet) {
            Contract.Requires(pclient != null & bet >= 0);

            var msg = new PirateMessage(PirateMessageHead.Pbet, bet.ToString());
            pclient.SendMessage(msg);
        }

        public static void NewRound(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Nrnd);

            var dealerName = PirateMessage.GetDealer(data);
            var round = PirateMessage.GetRound(data);

            if (pclient.Game.CurrentRound + 1 != round) return;
            Console.WriteLine("Starting new round: " + round + " - Dealer is " + dealerName);

            pclient.Game.NewRound();
            if(dealerName == pclient.Name) {
                pclient.DealCards();
            }
        }

        public static void BeginRound(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Bgrn);
            var bets = PirateMessage.GetPlayerBets(data);
            var round = PirateMessage.GetRound(data);

            if (pclient.Game.CurrentRound != round) return;

            Console.WriteLine("Player bets:");
            foreach(var kvp in bets) {
                pclient.Game.Round.PlayerBet(kvp.Key, kvp.Value);
                Console.WriteLine("\t" + kvp.Key + ": " + kvp.Value);
            }

            if (!pclient.Game.Round.BetsDone) return;
            Console.WriteLine("Round " + round + " has begun.");

            pclient.Game.Round.Begin();
        }

        public static void NewPile(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Trdn);

            var winner = PirateMessage.GetWinner(data);
            var tricks = PirateMessage.GetPlayerTricks(data);

            Console.WriteLine((winner == pclient.Name ? "You" : winner) + " won the trick!");
            Console.WriteLine("Tricks:");
            foreach(var kvp in tricks) {
                Console.WriteLine("\t" + kvp.Key + ": " + kvp.Value);
            }
        }

        public static void FinishRound(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Frnd);
            var scores = PirateMessage.GetPlayerScores(data);

            if (!pclient.Game.Round.Finished) {
                return;
            }

            Console.WriteLine("Round " + pclient.Game.CurrentRound + " finished - Scores:");
            foreach(var kvp in scores) {
                Console.WriteLine("\t" + kvp.Key + ": " + kvp.Value);
            }
        }
    }
}
