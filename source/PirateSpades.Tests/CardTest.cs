// <copyright file="CardTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.GameLogicV2;

namespace PirateSpades.GameLogicV2
{
    using PirateSpades.GameLogic;

    [TestClass]
    [PexClass(typeof(Card))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class CardTest
    {
        [PexMethod]
        public string ToString01([PexAssumeUnderTest]Card target) {
            string result = target.ToString();
            return result;
            // TODO: add assertions to method CardTest.ToString01(Card)
        }
        [PexMethod]
        public string ToShortString([PexAssumeUnderTest]Card target) {
            string result = target.ToShortString();
            return result;
            // TODO: add assertions to method CardTest.ToShortString(Card)
        }
        [PexMethod]
        public bool SameSuit([PexAssumeUnderTest]Card target, Card card) {
            bool result = target.SameSuit(card);
            return result;
            // TODO: add assertions to method CardTest.SameSuit(Card, Card)
        }
        [PexMethod]
        public bool HigherThan([PexAssumeUnderTest]Card target, Card card) {
            bool result = target.HigherThan(card);
            return result;
            // TODO: add assertions to method CardTest.HigherThan(Card, Card)
        }
        [PexMethod]
        public int GetHashCode01([PexAssumeUnderTest]Card target) {
            int result = target.GetHashCode();
            return result;
            // TODO: add assertions to method CardTest.GetHashCode01(Card)
        }
        [PexMethod]
        public Card FromString(string s) {
            Card result = Card.FromString(s);
            return result;
            // TODO: add assertions to method CardTest.FromString(String)
        }
        [PexMethod]
        public bool Equals01([PexAssumeUnderTest]Card target, object obj) {
            bool result = target.Equals(obj);
            return result;
            // TODO: add assertions to method CardTest.Equals01(Card, Object)
        }
        [PexMethod]
        public string EnumRegexString(Type enumType) {
            string result = Card.EnumRegexString(enumType);
            return result;
            // TODO: add assertions to method CardTest.EnumRegexString(Type)
        }
        [PexMethod]
        public int CompareTo([PexAssumeUnderTest]Card target, object obj) {
            int result = target.CompareTo(obj);
            return result;
            // TODO: add assertions to method CardTest.CompareTo(Card, Object)
        }
        [PexMethod]
        public int CardsToDeal(int round, int players) {
            int result = Card.CardsToDeal(round, players);
            return result;
            // TODO: add assertions to method CardTest.CardsToDeal(Int32, Int32)
        }
        [PexMethod]
        public Card Constructor(Suit suit, CardValue value) {
            Card target = new Card(suit, value);
            return target;
            // TODO: add assertions to method CardTest.Constructor(Suit, CardValue)
        }
    }
}
