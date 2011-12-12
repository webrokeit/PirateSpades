// <copyright file="PirateHostCommands.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      Various commands used by the PirateHost to communicate with its clients (PirateClient).
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;

    using PirateSpades.GameLogic;
    using PirateSpades.Misc;

    public class PirateHostCommands {
        private const string WelcomePhrase = "YARRR!!";

        public static void KnockKnock(PirateHost host, PirateClient pclient) {
            Contract.Requires(host != null && pclient != null);
            var msg = new PirateMessage(PirateMessageHead.Knck, "");
            host.SendMessage(pclient, msg);
        }

        public static void InitConnection(PirateHost host, PirateClient pclient, PirateMessage data) {
            Contract.Requires(host != null && pclient != null && data != null && data.Head == PirateMessageHead.Init);
            var msg = new PirateMessage(PirateMessageHead.Init, WelcomePhrase);
            host.SendMessage(pclient, msg);
        }

        public static void VerifyConnection(PirateHost host, PirateClient pclient, PirateMessage data) {
            Contract.Requires(host != null && pclient != null && data != null && data.Head == PirateMessageHead.Verf);
            if(data.Body == WelcomePhrase) {
                GetPlayerInfo(host, pclient);
            }
        }

        public static void ErrorMessage(PirateHost host, PirateClient pclient, PirateError error) {
            Contract.Requires(host != null && pclient != null && error != PirateError.Unknown);
            var msg = new PirateMessage(PirateMessageHead.Erro, error.ToString());
            host.SendMessage(pclient, msg);
        }

        public static void GetPlayerInfo(PirateHost host, PirateClient pclient) {
            Contract.Requires(host != null && pclient != null);
            var msg = new PirateMessage(PirateMessageHead.Pnfo, "");
            host.SendMessage(pclient, msg);
        }

        public static void SetPlayerInfo(PirateHost host, PirateClient pclient, PirateMessage data) {
            Contract.Requires(host != null && pclient != null && data != null && data.Head == PirateMessageHead.Pnfo);
            var player = PirateMessage.GetPlayerName(data);
            if (player == null) return;

            if (!host.ContainsPlayer(player)) {
                if(host.Game.Contains(pclient)) host.Game.RemovePlayer(pclient);
                host.SetPlayerName(pclient, player);
                host.Game.ClearPlayers();
                host.Game.AddPlayers(host.GetPlayers());
                pclient.SetGame(host.Game);
                SendPlayerInfo(host);
            }else {
                var msg = new PirateMessage(PirateMessageHead.Erro, PirateError.NameAlreadyTaken.ToString());
                host.SendMessage(pclient, msg);
            }
        }

        public static void SendPlayerInfo(PirateHost host) {
            Contract.Requires(host != null);

            var msg = new PirateMessage(PirateMessageHead.Pigm, PirateMessage.ConstructBody(host.GetPlayers().Select(player => player.ToString()).ToArray()));

            if (host.PlayerCount > 0) {
                if(host.DebugMode) Console.WriteLine("Host: Players in game:");
                foreach (var player in host.GetPlayers()) {
                    if(host.DebugMode) Console.WriteLine("\t" + player.Name);
                    host.SendMessage(player, msg);
                }
            }
        }

        public static void StartGame(PirateHost host) {
            Contract.Requires(host != null && host.PlayerCount >= 2);
            host.StopAccepting();

            var dealerIndex = CollectionFnc.PickRandom(0, host.Game.Players.Count - 1);
            Console.WriteLine("Starting player is: " + host.Game.Players[dealerIndex].Name);

            var msg = new PirateMessage(PirateMessageHead.Gstr, PirateMessage.ConstructStartingPlayer(host.Game.Players[dealerIndex]));
            foreach(var pclient in host.GetPlayers()) {
                host.SendMessage(pclient, msg);
            }

            host.Game.Start(true, dealerIndex);
        }

        public static void GameFinished(PirateHost host) {
            Contract.Requires(host != null);

            var body =
                PirateMessage.ConstructBody(
                    PirateMessage.ContstructPlayerScores(host.Game.GetTotalScores()));
            body = PirateMessage.AppendBody(body, PirateMessage.ConstructWinner(host.Game.Leader));
            var msg = new PirateMessage(PirateMessageHead.Gfin, body);

            foreach (var player in host.GetPlayers()) {
                host.SendMessage(player, msg);
            }
        }

        public static void DealCard(PirateHost host, PirateMessage data) {
            Contract.Requires(host != null && data != null && data.Head == PirateMessageHead.Xcrd);
            var player = PirateMessage.GetPlayerName(data);
            if(player == null) {
                return;
            }

            var pclient = host.PlayerFromString(player);
            if(pclient == null) {
                return;
            }

            var card = Card.FromString(data.Body);
            if(card == null) {
                return;
            }

            pclient.GetCard(card);

            Console.WriteLine("Host: Sending card " + card.ToShortString() + " to " + pclient);

            var msg = new PirateMessage(PirateMessageHead.Xcrd, card.ToString());
            host.SendMessage(pclient, msg);

            if(host.Game.Round.CardsDealt == host.Game.Round.TotalCards) {
                RequestBets(host);
            }
        }

        public static void RequestCard(PirateHost host, PirateClient pclient) {
            Contract.Requires(host != null && pclient != null);
            Console.WriteLine("Sending card request to " + pclient.Name);
            var msg = new PirateMessage(PirateMessageHead.Creq, "");
            host.SendMessage(pclient, msg);
        }

        public static void PlayCard(PirateHost host, PirateMessage data) {
            Contract.Requires(host != null && data != null && data.Head == PirateMessageHead.Pcrd);
            var playerName = PirateMessage.GetPlayerName(data);
            var player = host.PlayerFromString(playerName);
            var card = Card.FromString(data.Body);

            if(!player.CardPlayable(card, host.Game.Round.BoardCards.FirstCard)) {
                ErrorMessage(host, player, PirateError.CardNotPlayable);
                var returnCard = new PirateMessage(PirateMessageHead.Xcrd, card.ToString());
                host.SendMessage(player, returnCard);
                RequestCard(host, player);
                return;
            }

            Console.WriteLine(player.Name + " plays " + card.ToShortString());

            var msg = new PirateMessage(
                PirateMessageHead.Pcrd, PirateMessage.ConstructBody(player.ToString(), card.ToString()));

            foreach(var pclient in host.GetPlayers()) {
                host.SendMessage(pclient, msg);
            }

            player.PlayCard(card);
            //host.Game.Round.PlayCard(player, card);
            if(!host.Game.Round.Finished) {
                RequestCard(host, host.PlayerFromIndex(host.Game.Round.CurrentPlayer));
            }else {
                host.Game.NewRound();
            }
        }

        public static void RequestBets(PirateHost host) {
            Contract.Requires(host != null);

            var msg = new PirateMessage(PirateMessageHead.Breq, "");
            foreach(var pclient in host.GetPlayers()) {
                host.SendMessage(pclient, msg);
            }
        }

        public static void ReceiveBet(PirateHost host, PirateClient player, PirateMessage msg) {
            Contract.Requires(host != null && player != null && msg != null && msg.Head == PirateMessageHead.Pbet && host.Game.Round.AwaitingBets);

            lock (host.Game.Round) {
                var bet = 0;
                if (int.TryParse(msg.Body, out bet)) {
                    player.SetBet(bet);
                } else {
                    ErrorMessage(host, player, PirateError.InvalidBet);
                }

                if (host.Game.Round.BetsDone) {
                    BeginRound(host);
                }
            }
        }

        public static void NewRound(PirateHost host) {
            Contract.Requires(host != null);

            var body =
                PirateMessage.ConstructBody(
                    PirateMessage.ConstructDealer(host.Game.Players[host.Game.Round.Dealer].Name),
                    PirateMessage.ConstructRoundNumber(host.Game.CurrentRound));
            var msg = new PirateMessage(PirateMessageHead.Nrnd, body);
            foreach (var pclient in host.GetPlayers()) {
                host.SendMessage(pclient, msg);
            }
            Console.WriteLine("Starting new round: " + host.Game.CurrentRound);
        }

        public static void BeginRound(PirateHost host) {
            Contract.Requires(host != null && host.Game.Round.BetsDone);

            var bets = new HashSet<string>();
            foreach(var player in host.GetPlayers()) {
                bets.Add(PirateMessage.ConstructPlayerBet(player));
            }
            bets.Add(PirateMessage.ConstructRoundNumber(host.Game.CurrentRound));

            var msg = new PirateMessage(PirateMessageHead.Bgrn, PirateMessage.ConstructBody(bets));

            foreach(var player in host.GetPlayers()) {
                host.SendMessage(player, msg);
            }
            lock (host.Game.Round) {
                host.Game.Round.Begin();
            }
        }

        public static void NewPile(PirateHost host) {
            Contract.Requires(host != null);

            var body = PirateMessage.ConstructBody(PirateMessage.ConstructPlayerTricks(host.Game.Round));
            body = PirateMessage.AppendBody(body, PirateMessage.ConstructWinner(host.Game.Round.LastTrick.Winner));
            var msg = new PirateMessage(PirateMessageHead.Trdn, body);

            foreach(var player in host.GetPlayers()) {
                host.SendMessage(player, msg);
            }
        }

        public static void RoundFinished(PirateHost host) {
            Contract.Requires(host != null);

            var body =
                PirateMessage.ConstructBody(
                    PirateMessage.ContstructPlayerScores(host.Game.GetRoundScoreTotal(host.Game.CurrentRound)));
            var msg = new PirateMessage(PirateMessageHead.Frnd, body);

            foreach (var player in host.GetPlayers()) {
                host.SendMessage(player, msg);
            }
        }
    }
}
