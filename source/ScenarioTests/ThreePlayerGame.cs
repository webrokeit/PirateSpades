using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioTests {
    using NUnit.Framework;

    using PirateSpades.Network;

    [TestFixture]
    public class ThreePlayerGame {
        [Test]
        public void PlayGame() {
            PirateHost host = new PirateHost(4939);

        }
    }
}
