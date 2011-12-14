using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioTests {
    using NUnit.Framework;

    using PirateSpades.GameLogic;
    using PirateSpades.Misc;
    using PirateSpades.Network;

    [TestFixture]
    public class ThreePlayerGame {
        [Test]
        public void PlayGame() {
            PirateHost host = new PirateHost(4939);
            host.Start();
            host.Broadcaster.Interval = 1.1;
            Assert.That(host.Broadcaster.Interval < 1.11 && host.Broadcaster.Interval > 1.09);

            PirateClient player1 = new PirateClient("Player1", "127.0.0.1", 4939);
            player1.SetGame(new Game());
            player1.BetRequested += OnBetRequest;
            player1.CardRequested += OnCardRequest;
            player1.Disconnected += OnDisconnect;
            player1.InitConnection();
            while(!host.ContainsPlayer(player1.Name)) {}

            Assert.That(PirateScanner.CheckIp(PirateScanner.GetIp("127.0.0.1"), 4939, 1000));

            PirateClient player2 = new PirateClient("Player2", "127.0.0.1", 4939);
            player2.SetGame(new Game());
            player2.BetRequested += OnBetRequest;
            player2.CardRequested += OnCardRequest;
            player2.Disconnected += OnDisconnect;
            player2.InitConnection();
            while(!host.ContainsPlayer(player2.Name)) {}
            Assert.That(host.ContainsPlayer(player1.Name));

            var ps = new PirateScanner();
            var gameinfos = ps.ScanForGames(4939, 2000);
            Assert.That(gameinfos.Count > 0);
            var gameinfo = gameinfos[0];

            PirateClient player3 = new PirateClient("Player3", gameinfo.Ip, 4939);
            player3.SetGame(new Game());
            player3.BetRequested += OnBetRequest;
            player3.CardRequested += OnCardRequest;
            player3.Disconnected += OnDisconnect;
            player3.InitConnection();
            while(!host.ContainsPlayer(player3.Name)) {}
            Assert.That(player1.Name == host.PlayerFromSocket(host.GetPlayers().First().Socket).Name);

            while(host.Game.Players.Count != 3) {}

            host.StartGame();

            while(!host.Game.Finished) {
                Assert.That(host.Game.Started);
            }

            Assert.That(host.Game.Finished);

            host.Stop();

            while(player1.Socket.Connected || player2.Socket.Connected || player3.Socket.Connected) {}
        }

        private static void OnBetRequest(PirateClient pclient) {
            var bet = CollectionFnc.PickRandom(0, pclient.Hand.Count);
            Assert.That(bet >= 0 && bet <= pclient.Game.CardsToDeal);
            pclient.SetBet(bet);
            Assert.That(pclient.Bet == bet);
        }

        private static void OnCardRequest(PirateClient pclient) {
            var c = pclient.GetPlayableCard();
            Assert.That(pclient.HasCard(c));
            pclient.PlayCard(pclient.GetPlayableCard());
            Assert.That(!pclient.HasCard(c));
        }

        private static void OnDisconnect(PirateClient pclient) {
            Assert.That(!pclient.Socket.Connected);
        }
    }
}
