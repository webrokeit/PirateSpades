// <copyright file="PirateClientTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Network;
using System.Collections.Generic;
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
        public string NameToString(string name) {
            string result = PirateClient.NameToString(name);
            return result;
            // TODO: add assertions to method PirateClientTest.NameToString(String)
        }
        [PexMethod]
        public HashSet<string> NamesFromString(string s) {
            HashSet<string> result = PirateClient.NamesFromString(s);
            return result;
            // TODO: add assertions to method PirateClientTest.NamesFromString(String)
        }
        [PexMethod]
        public string NameFromString(string s) {
            string result = PirateClient.NameFromString(s);
            return result;
            // TODO: add assertions to method PirateClientTest.NameFromString(String)
        }
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
