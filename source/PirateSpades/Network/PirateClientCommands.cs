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

    /// <summary>
    /// Various commands used by the PirateClientt to communicate with its host (PirateHost).
    /// </summary>
    public class PirateClientCommands {
        /// <summary>
        /// Check if host is available.
        /// </summary>
        /// <param name="socket">The socket to communicate through.</param>
        /// <returns>True if host is available, false if not.</returns>
        public static bool KnockKnock(Socket socket) {
            Contract.Requires(socket != null);
            var knock = new PirateMessage(PirateMessageHead.Knck, "");
            socket.Send(knock.GetBytes());
            var buffer = new byte[PirateMessage.BufferSize];
            var read = socket.Receive(buffer);
            return read > 4 && PirateMessage.GetMessages(buffer, read).Any(msg => msg.Head == PirateMessageHead.Knck);
        }

        /// <summary>
        /// Receive an error message.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="msg">Message to send.</param>
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

        /// <summary>
        /// Initialize connection.
        /// </summary>
        /// <param name="pclient">The client.</param>
        public static void InitConnection(PirateClient pclient) {
            Contract.Requires(pclient != null);
            var msg = new PirateMessage(PirateMessageHead.Init, "");
            pclient.SendMessage(msg);
        }

        /// <summary>
        /// Verify connection with host.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="data">Data received from host.</param>
        public static void VerifyConnection(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Init);
            var msg = new PirateMessage(PirateMessageHead.Verf, data.Body);
            pclient.SendMessage(msg);
        }

        /// <summary>
        /// Send player information.
        /// </summary>
        /// <param name="pclient">The client.</param>
        public static void SendPlayerInfo(PirateClient pclient) {
            Contract.Requires(pclient != null);
            var msg = new PirateMessage(PirateMessageHead.Pnfo, pclient.ToString());
            pclient.SendMessage(msg);
        }

        /// <summary>
        /// Get players in game.
        /// </summary>
        /// <param name="pclient">Client receiving the data.</param>
        /// <param name="data">Data received from host.</param>
        public static void GetPlayersInGame(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Pigm);

            pclient.Game.ClearPlayers();
            var players = PirateMessage.GetPlayerNames(data);
            if(players.Count > 0) {
                if(!pclient.VirtualPlayer)
                    Console.WriteLine("Current players in game:");
               foreach(var player in players) {
                   var p = pclient.Name == player ? pclient : new Player(player);
                   p.SetGame(pclient.Game);
                   pclient.Game.AddPlayer(p);
                   if(!pclient.VirtualPlayer)
                       Console.WriteLine("\t" + player + (pclient.Name == player ? " (YOU)" : ""));
               }
            }
        }

        /// <summary>
        /// Game has been started,
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="data">Data received from host.</param>
        public static void GameStarted(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Gstr);

            var startPlayer = PirateMessage.GetStartingPlayer(data);
            if (startPlayer == null) return;

            if (!pclient.Game.Contains(startPlayer)) return;

            pclient.Game.Start(pclient.Game.PlayerIndex(startPlayer));
        }

        /// <summary>
        /// Game has finished.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="data">Data received from host.</param>
        public static void GameFinished(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Gfin);

            var scores = PirateMessage.GetPlayerScores(data);
            var winner = PirateMessage.GetWinner(data);
            
            pclient.Game.NewRound();
            if(!pclient.VirtualPlayer) {
                Console.WriteLine("GAME FINISHED - Scores:");
                foreach (var kvp in scores) {
                    Console.WriteLine("\t" + kvp.Key + ": " + kvp.Value + (kvp.Key == winner ? " [WINNER!!!]" : ""));
                }
            }
        }

        /// <summary>
        /// Play a card.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="card">Card to be played.</param>
        public static void PlayCard(PirateClient pclient, Card card) {
            Contract.Requires(pclient != null && card != null);
            var body = PirateMessage.ConstructBody(pclient.ToString(), card.ToString());
            var msg = new PirateMessage(PirateMessageHead.Pcrd, body);
            pclient.SendMessage(msg);
        }

        /// <summary>
        /// Receive a card played.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="data">Data received from host.</param>
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

            if(!pclient.VirtualPlayer)
                Console.WriteLine(player.Name + " plays " + card.ToShortString());
        }

        /// <summary>
        /// Deal card to player.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="receiver">Receiving player.</param>
        /// <param name="card">Card dealt.</param>
        public static void DealCard(PirateClient pclient, Player receiver, Card card) {
            Contract.Requires(pclient != null && receiver != null && card != null);
            var body = PirateMessage.ConstructBody(PirateMessage.ConstructPlayerName(receiver.Name), card.ToString());
            var msg = new PirateMessage(PirateMessageHead.Xcrd, body);

            if(!pclient.VirtualPlayer)
                Console.WriteLine(pclient.Name + ": Dealing " + card.ToShortString() + " to " + receiver.Name);
            pclient.SendMessage(msg);
        }

        /// <summary>
        /// Get dealt card.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="data">Data received from host.</param>
        public static void GetCard(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Xcrd);
            var card = Card.FromString(data.Body);
            if(card == null) return;

            if(!pclient.VirtualPlayer)
                Console.WriteLine(pclient.Name + ": Received " + card.ToShortString());
            if (!pclient.IsDealer) {
                pclient.GetCard(card);
            }
        }

        /// <summary>
        /// Set the bet.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="bet">Bet to use.</param>
        public static void SetBet(PirateClient pclient, int bet) {
            Contract.Requires(pclient != null & bet >= 0);

            var msg = new PirateMessage(PirateMessageHead.Pbet, bet.ToString());
            pclient.SendMessage(msg);
        }

        /// <summary>
        /// A new round has been started.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="data">Data received from host.</param>
        public static void NewRound(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Nrnd);

            var dealerName = PirateMessage.GetDealer(data);
            var round = PirateMessage.GetRound(data);

            if (pclient.Game.CurrentRound + 1 != round) return;
            if(!pclient.VirtualPlayer)
                Console.WriteLine("Starting new round: " + round + " - Dealer is " + dealerName);

            pclient.Game.NewRound();
            if(dealerName == pclient.Name) {
                pclient.DealCards();
            }
        }

        /// <summary>
        /// A round has begun.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="data">Data received from host.</param>
        public static void BeginRound(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Bgrn);
            var bets = PirateMessage.GetPlayerBets(data);
            var round = PirateMessage.GetRound(data);

            if (pclient.Game.CurrentRound != round) return;

            if(!pclient.VirtualPlayer)
                Console.WriteLine("Player bets:");
            foreach(var kvp in bets) {
                pclient.Game.Round.PlayerBet(kvp.Key, kvp.Value);
                if(!pclient.VirtualPlayer)
                    Console.WriteLine("\t" + kvp.Key + ": " + kvp.Value);
            }

            if (!pclient.Game.Round.BetsDone) return;
            if(!pclient.VirtualPlayer)
                Console.WriteLine("Round " + round + " has begun.");

            pclient.Game.Round.Begin();
        }

        /// <summary>
        /// A new pile has been created.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="data">Data received from host.</param>
        public static void NewPile(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Trdn);

            var winner = PirateMessage.GetWinner(data);
            var tricks = PirateMessage.GetPlayerTricks(data);

            if(!pclient.VirtualPlayer) {
                Console.WriteLine((winner == pclient.Name ? "You" : winner) + " won the trick!");
                Console.WriteLine("Tricks:");
                foreach (var kvp in tricks) {
                    Console.WriteLine("\t" + kvp.Key + ": " + kvp.Value);
                }
            }
        }

        /// <summary>
        /// A round has finished.
        /// </summary>
        /// <param name="pclient">The client.</param>
        /// <param name="data">Data received from host.</param>
        public static void FinishRound(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Frnd);
            var scores = PirateMessage.GetPlayerScores(data);

            if (!pclient.Game.Round.Finished) {
                return;
            }
            if(!pclient.VirtualPlayer) {
                Console.WriteLine("Round " + pclient.Game.CurrentRound + " finished - Scores:");
                foreach (var kvp in scores) {
                    Console.WriteLine("\t" + kvp.Key + ": " + kvp.Value);
                }
            }
        }
    }
}
