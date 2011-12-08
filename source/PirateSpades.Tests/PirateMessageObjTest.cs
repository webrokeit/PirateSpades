// <copyright file="PirateMessageObjTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Network;

namespace PirateSpades.Network
{
    [TestClass]
    [PexClass(typeof(PirateMessageObj))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PirateMessageObjTest
    {
        [PexMethod]
        public PirateMessageObj Constructor(PirateClient client) {
            PirateMessageObj target = new PirateMessageObj(client);
            return target;
            // TODO: add assertions to method PirateMessageObjTest.Constructor(PirateClient)
        }
    }
}
