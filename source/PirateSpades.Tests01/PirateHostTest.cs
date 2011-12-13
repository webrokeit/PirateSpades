// <copyright file="PirateHostTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Network;

namespace PirateSpades.Network
{
    [TestClass]
    [PexClass(typeof(PirateHost))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PirateHostTest
    {
        [PexMethod]
        public bool IsValidGameName(string gameName) {
            bool result = PirateHost.IsValidGameName(gameName);
            return result;
            // TODO: add assertions to method PirateHostTest.IsValidGameName(String)
        }
        [PexMethod]
        public PirateHost Constructor(int port) {
            PirateHost target = new PirateHost(port);
            return target;
            // TODO: add assertions to method PirateHostTest.Constructor(Int32)
        }
    }
}
