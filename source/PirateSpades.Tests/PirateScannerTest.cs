// <copyright file="PirateScannerTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Network;
using System.Net;
using System.Collections.Generic;

namespace PirateSpades.Network
{
    [TestClass]
    [PexClass(typeof(PirateScanner))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PirateScannerTest
    {
        [PexMethod]
        public IList<IPAddress> ScanForIps(
            [PexAssumeUnderTest]PirateScanner target,
            int port,
            int timeout,
            int amount
        ) {
            IList<IPAddress> result = target.ScanForIps(port, timeout, amount);
            return result;
            // TODO: add assertions to method PirateScannerTest.ScanForIps(PirateScanner, Int32, Int32, Int32)
        }
        [PexMethod]
        public IPAddress ScanForIp([PexAssumeUnderTest]PirateScanner target, int port) {
            IPAddress result = target.ScanForIp(port);
            return result;
            // TODO: add assertions to method PirateScannerTest.ScanForIp(PirateScanner, Int32)
        }
        [PexMethod]
        public double PercentageDoneGet([PexAssumeUnderTest]PirateScanner target) {
            double result = target.PercentageDone;
            return result;
            // TODO: add assertions to method PirateScannerTest.PercentageDoneGet(PirateScanner)
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
