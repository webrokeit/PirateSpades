// <copyright file="PirateMessageTest.GetError.g.cs">Copyright �  2011</copyright>
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace PirateSpades.Network
{
    public partial class PirateMessageTest {
[TestMethod]
[PexGeneratedBy(typeof(PirateMessageTest))]
public void GetError632()
{
    PirateError i;
    i = this.GetError((string)null);
    Assert.AreEqual<PirateError>(PirateError.Unknown, i);
}
    }
}
