// <copyright file="DeckTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.GameLogicV2;

namespace PirateSpades.GameLogicV2
{
    [TestClass]
    [PexClass(typeof(Deck))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class DeckTest
    {
        [PexMethod]
        public Deck GetShuffledDeck() {
            Deck result = Deck.GetShuffledDeck();
            return result;
            // TODO: add assertions to method DeckTest.GetShuffledDeck()
        }
    }
}
