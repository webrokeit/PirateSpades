// <copyright file="PirateBroadcasterTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Network;

namespace PirateSpades.Network
{
    [TestClass]
    [PexClass(typeof(PirateBroadcaster))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PirateBroadcasterTest
    {
        [PexMethod]
        public void IntervalSet([PexAssumeUnderTest]PirateBroadcaster target, double value) {
            target.Interval = value;
            // TODO: add assertions to method PirateBroadcasterTest.IntervalSet(PirateBroadcaster, Double)
        }
        [PexMethod]
        public double IntervalGet([PexAssumeUnderTest]PirateBroadcaster target) {
            double result = target.Interval;
            return result;
            // TODO: add assertions to method PirateBroadcasterTest.IntervalGet(PirateBroadcaster)
        }
        [PexMethod]
        public void Stop([PexAssumeUnderTest]PirateBroadcaster target) {
            target.Stop();
            // TODO: add assertions to method PirateBroadcasterTest.Stop(PirateBroadcaster)
        }
        [PexMethod]
        public void Start([PexAssumeUnderTest]PirateBroadcaster target) {
            target.Start();
            // TODO: add assertions to method PirateBroadcasterTest.Start(PirateBroadcaster)
        }
        [PexMethod]
        public PirateBroadcaster Constructor(
            byte[] message,
            int port,
            double interval
        ) {
            PirateBroadcaster target = new PirateBroadcaster(message, port, interval);
            return target;
            // TODO: add assertions to method PirateBroadcasterTest.Constructor(Byte[], Int32, Double)
        }
    }
}
