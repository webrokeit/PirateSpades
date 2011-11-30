using NUnit.Framework;
using System.Collections.Generic;
using PirateSpades.GameLogic;

namespace PirateSpadesTest {

    [TestFixture]
    public class TestPlayer {
        [Test]
        public void TestDealing() {
            var p = new Player("Jens");
            Assert.That(p.Name == "Jens");
            Assert.That(!p.IsDealer);
            p.IsDealer = true;
            Assert.That(p.IsDealer);
            var p2 = new Player("Alberto");
            var p3 = new Player("Andreas");
            var players = new List<Player> { p2, p3, p };
            p.DealCards(players, 10);
            Assert.That(p.NumberOfCards == 10);
            Assert.That(p2.NumberOfCards == 10);
            Assert.That(p3.NumberOfCards == 10);
        }

        [Test]
        public void TestWithCards() {
            var p = new Player("Jens");
            var c = new Card(Suit.Spades, CardValue.Ace);
            Assert.That(!p.IsDealer);
            p.ReceiveCard(c);
            Assert.That(p.HaveCard(c));
            Assert.That(p.Hand(0) == c);
            Assert.That(p.AnyCard(Suit.Spades));
            Assert.That(!p.AnyCard(Suit.Clubs));
            Assert.That(!p.AnyCard(Suit.Diamonds));
            Assert.That(!p.AnyCard(Suit.Hearts));
            Assert.That(p.Tricks == 0);
            var c2 = new Card(Suit.Clubs, CardValue.Ace);
            var c3 = new Card(Suit.Hearts, CardValue.Eight);
            var c4 = new Card(Suit.Diamonds, CardValue.Five);
            var c5 = new Card(Suit.Spades, CardValue.Seven);
            var cards = new List<Card>() { c2, c3, c4, c5 };
            p.ReceiveTrick(cards);
            Assert.That(p.Tricks > 0);
            Assert.That(p.Tricks == 1);
            p.ClearTricks();
            Assert.That(p.Tricks == 0);
            p.Bet = 2;
            Assert.That(p.Bet == 2);
        }
    }
}
