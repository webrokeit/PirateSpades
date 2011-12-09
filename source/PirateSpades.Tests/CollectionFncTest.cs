// <copyright file="CollectionFncTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Misc;
using System.Collections.Generic;

namespace PirateSpades.Misc
{
    [TestClass]
    [PexClass(typeof(CollectionFnc))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class CollectionFncTest
    {
        [PexMethod]
        public int PickRandom03(int min, int max) {
            int result = CollectionFnc.PickRandom(min, max);
            return result;
            // TODO: add assertions to method CollectionFncTest.PickRandom03(Int32, Int32)
        }
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public T PickRandom02<T>(List<T> c) {
            T result = CollectionFnc.PickRandom<T>(c);
            return result;
            // TODO: add assertions to method CollectionFncTest.PickRandom02(List`1<!!0>)
        }
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public T PickRandom01<T>(IEnumerable<T> c) {
            T result = CollectionFnc.PickRandom<T>(c);
            return result;
            // TODO: add assertions to method CollectionFncTest.PickRandom01(IEnumerable`1<!!0>)
        }
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public T PickRandom<T>(ICollection<T> c) {
            T result = CollectionFnc.PickRandom<T>(c);
            return result;
            // TODO: add assertions to method CollectionFncTest.PickRandom(ICollection`1<!!0>)
        }
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public void FisherYatesShuffle<T>(List<T> l) {
            CollectionFnc.FisherYatesShuffle<T>(l);
            // TODO: add assertions to method CollectionFncTest.FisherYatesShuffle(List`1<!!0>)
        }
    }
}
