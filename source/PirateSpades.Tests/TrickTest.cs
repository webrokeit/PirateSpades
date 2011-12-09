// <copyright file="TrickTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.GameLogicV2;

namespace PirateSpades.GameLogicV2
{
    [TestClass]
    [PexClass(typeof(Trick))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class TrickTest
    {
        [PexMethod]
        public Trick Constructor() {
            Trick target = new Trick();
            return target;
            // TODO: add assertions to method TrickTest.Constructor()
        }
    }
}
