using NUnit.Framework;
using System.Collections.Generic;
using PirateSpades.GameLogic;

namespace PirateSpadesTest {
    [TestFixture]
    public class TestRound {
        [Test]
        public void Simulate() {
            var p = new Player("Hans");
            var p2 = new Player("Jens");
            var p3 = new Player("Alberto");
            var players = new List<Player>() { p, p2, p3 };
            var r = new Round(players, 10, 1);
            Assert.That(r.PlayerCards == 10);
            Assert.That(r.Players == 3);
            Assert.That(r.TotalCards == (r.Players * r.PlayerCards));
            Assert.That(!r.IsFinished());
            Assert.That(r.DealingPlayer().IsDealer);
            p.Bet = 3;
            p2.Bet = 4;
            p3.Bet = 3;
            r.CollectBet(p, p.Bet);
            Assert.That(r.HasPlayerBet(p));
            Assert.That(r.PlayerBet(p) == p.Bet);
            r.CollectBet(p2, p2.Bet);
            Assert.That(r.HasPlayerBet(p2));
            Assert.That(r.PlayerBet(p2) == p2.Bet);
            r.CollectBet(p3, p3.Bet);
            Assert.That(r.HasPlayerBet(p3));
            Assert.That(r.PlayerBet(p3) == p3.Bet);
            r.Start();
        }
    }
}
