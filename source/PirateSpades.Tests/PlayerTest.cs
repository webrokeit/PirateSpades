// <copyright file="PlayerTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.GameLogicV2;

namespace PirateSpades.GameLogicV2
{
    [TestClass]
    [PexClass(typeof(Player))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PlayerTest
    {
        [PexMethod]
        public Player Constructor(string name) {
            Player target = new Player(name);
            return target;
            // TODO: add assertions to method PlayerTest.Constructor(String)
        }
    }
}
