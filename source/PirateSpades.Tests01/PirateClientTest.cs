// <copyright file="PirateClientTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Network;
using System.Net;
using System.Net.Sockets;

namespace PirateSpades.Network
{
    [TestClass]
    [PexClass(typeof(PirateClient))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PirateClientTest
    {
        [PexMethod]
        public PirateClient Constructor02(
            string name,
            string ip,
            int port
        ) {
            PirateClient target = new PirateClient(name, ip, port);
            return target;
            // TODO: add assertions to method PirateClientTest.Constructor02(String, String, Int32)
        }
        [PexMethod]
        public PirateClient Constructor01(
            string name,
            IPAddress ip,
            int port
        ) {
            PirateClient target = new PirateClient(name, ip, port);
            return target;
            // TODO: add assertions to method PirateClientTest.Constructor01(String, IPAddress, Int32)
        }
        [PexMethod]
        public PirateClient Constructor(Socket socket) {
            PirateClient target = new PirateClient(socket);
            return target;
            // TODO: add assertions to method PirateClientTest.Constructor(Socket)
        }
    }
}
