// <copyright file="RoundTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.GameLogicV2;

namespace PirateSpades.GameLogicV2
{
    [TestClass]
    [PexClass(typeof(Round))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class RoundTest
    {
        [PexMethod]
        public int RoundsPossible(int players) {
            int result = Round.RoundsPossible(players);
            return result;
            // TODO: add assertions to method RoundTest.RoundsPossible(Int32)
        }
        [PexMethod]
        public Round Constructor(Game game, int dealer) {
            Round target = new Round(game, dealer);
            return target;
            // TODO: add assertions to method RoundTest.Constructor(Game, Int32)
        }
    }
}
