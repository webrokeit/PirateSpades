// <copyright file="PirateScannerTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Network;
using System.Collections.Generic;
using System.Net;

namespace PirateSpades.Network
{
    [TestClass]
    [PexClass(typeof(PirateScanner))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PirateScannerTest
    {
        [PexMethod]
        public IList<PirateScanner.GameInfo> ScanForGames(
            [PexAssumeUnderTest]PirateScanner target,
            int port,
            int timeout
        ) {
            IList<PirateScanner.GameInfo> result = target.ScanForGames(port, timeout);
            return result;
            // TODO: add assertions to method PirateScannerTest.ScanForGames(PirateScanner, Int32, Int32)
        }
        [PexMethod]
        public bool IsValidIp01(IPAddress ip) {
            bool result = PirateScanner.IsValidIp(ip);
            return result;
            // TODO: add assertions to method PirateScannerTest.IsValidIp01(IPAddress)
        }
        [PexMethod]
        public bool IsValidIp(string ip) {
            bool result = PirateScanner.IsValidIp(ip);
            return result;
            // TODO: add assertions to method PirateScannerTest.IsValidIp(String)
        }
        [PexMethod]
        public IPAddress GetLocalIpV4() {
            IPAddress result = PirateScanner.GetLocalIpV4();
            return result;
            // TODO: add assertions to method PirateScannerTest.GetLocalIpV4()
        }
        [PexMethod]
        public IEnumerable<IPAddress> GetLocalIpsV4() {
            IEnumerable<IPAddress> result = PirateScanner.GetLocalIpsV4();
            return result;
            // TODO: add assertions to method PirateScannerTest.GetLocalIpsV4()
        }
        [PexMethod]
        public IPAddress GetIp(string ip) {
            IPAddress result = PirateScanner.GetIp(ip);
            return result;
            // TODO: add assertions to method PirateScannerTest.GetIp(String)
        }
        [PexMethod]
        public PirateScanner Constructor() {
            PirateScanner target = new PirateScanner();
            return target;
            // TODO: add assertions to method PirateScannerTest.Constructor()
        }
        [PexMethod]
        public bool CheckIp(
            IPAddress ip,
            int port,
            int timeout
        ) {
            bool result = PirateScanner.CheckIp(ip, port, timeout);
            return result;
            // TODO: add assertions to method PirateScannerTest.CheckIp(IPAddress, Int32, Int32)
        }
    }
}
