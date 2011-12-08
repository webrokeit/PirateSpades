// <copyright file="CardTest.ToString01.g.cs">Copyright �  2011</copyright>
// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using System;
using Microsoft.Pex.Framework.Explorable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace PirateSpades.GameLogicV2
{
    public partial class CardTest {
[TestMethod]
[PexGeneratedBy(typeof(CardTest))]
public void ToString0157()
{
    Card card;
    string s;
    card = PexInvariant.CreateInstance<Card>();
    PexInvariant.SetField<CardValue>
        ((object)card, "<Value>k__BackingField", CardValue.Two);
    PexInvariant.SetField<Suit>
        ((object)card, "<Suit>k__BackingField", Suit.Diamonds);
    PexInvariant.CheckInvariant((object)card);
    s = this.ToString01(card);
    Assert.AreEqual<string>("card: Diamonds;Two", s);
    Assert.IsNotNull((object)card);
    Assert.AreEqual<CardValue>(CardValue.Two, card.Value);
    Assert.AreEqual<Suit>(Suit.Diamonds, card.Suit);
}
    }
}
