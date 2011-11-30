using NUnit.Framework;
using System.Collections.Generic;
using PirateSpades.GameLogic;

namespace PirateSpadesTest {

    [TestFixture]
    public class TestTable {
        [Test]
        public void SimulateRound() {
            Table t = Table.GetTable();
            var p = new Player("Hans");
            var p2 = new Player("Jens");
            var p3 = new Player("Alberto");
            var players = new List<Player>() { p, p2, p3 };
            t.AddPlayers(players);
            Assert.That(t.Players == 3);
            Assert.That(t.Cards == 0);
            t.StartingPlayer = p3;
            t.PlayerTurn = p3;
            Assert.That(t.StartingPlayer == p3);
            Assert.That(t.PlayerTurn == p3);
            var c = new Card(Suit.Spades, CardValue.Ace);
            var c2 = new Card(Suit.Hearts, CardValue.Eight);
            var c3 = new Card(Suit.Clubs, CardValue.Four);
            var c4 = new Card(Suit.Hearts, CardValue.Ten);
            var c5 = new Card(Suit.Diamonds, CardValue.Six);
            var c6 = new Card(Suit.Hearts, CardValue.Three);
            p.ReceiveCard(c);
            p.ReceiveCard(c4);
            p2.ReceiveCard(c2);
            p2.ReceiveCard(c5);
            p3.ReceiveCard(c3);
            p3.ReceiveCard(c6);
            Assert.That(!t.IsRoundFinished());
            p3.PlayCard(c6);
            Assert.That(!p3.HaveCard(c6));
            p.PlayCard(c4);
            Assert.That(!p.HaveCard(c4));
            p2.PlayCard(c2);
            Assert.That(!p2.HaveCard(c2));
            Assert.That(p.Tricks == 1);
            Assert.That(p2.Tricks == 0);
            Assert.That(p3.Tricks == 0);
            Assert.That(t.PlayerTurn == p);
            Assert.That(!t.IsRoundFinished());
            p.PlayCard(c);
            Assert.That(!p.HaveCard(c));
            p2.PlayCard(c5);
            Assert.That(!p2.HaveCard(c5));
            p3.PlayCard(c3);
            Assert.That(!p3.HaveCard(c3));
            Assert.That(p.Tricks == 2);
            Assert.That(p2.Tricks == 0);
            Assert.That(p3.Tricks == 0);
            Assert.That(!t.IsRoundFinished());
        }
    }
}
