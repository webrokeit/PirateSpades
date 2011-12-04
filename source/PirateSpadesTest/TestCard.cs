using NUnit.Framework;
using System.Collections.Generic;
using PirateSpades.GameLogic;

namespace PirateSpadesTest {
    using System;

    [TestFixture]
    public class TestCard {
        [Test]
        public void TestMethods() {
            var c = new Card(Suit.Spades, CardValue.Ace);
            Assert.That(c.Suit == Suit.Spades);
            Assert.That(c.Value == CardValue.Ace);
            var c2 = new Card(Suit.Hearts, CardValue.Eight);
            Assert.That(!c.SameSuit(c2));
            Assert.That(c.CompareTo(c2) > 0);
            Assert.That(c2.CompareTo(c) < 0);
            var c3 = new Card(Suit.Spades, CardValue.Four);
            Assert.That(c3.CompareTo(c2) > 0);
            Assert.That(c2.CompareTo(c3) < 0);
            Assert.That(c.CompareTo(c3) > 0);
            Assert.That(c3.CompareTo(c) < 0);
            Assert.That(c.SameSuit(c3));
        }
    }
}
