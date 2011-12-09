// <copyright file="PirateClientTest.NameFromString.g.cs">Copyright �  2011</copyright>
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
using Microsoft.Pex.Engine.Exceptions;

namespace PirateSpades.Network
{
    public partial class PirateClientTest {
[TestMethod]
[PexGeneratedBy(typeof(PirateClientTest))]
public void NameFromString537()
{
    string s;
    s = this.NameFromString("player_name: \u00ba\u2090\u0400");
    Assert.AreEqual<string>("\u00ba\u2090\u0400", s);
}
[TestMethod]
[PexGeneratedBy(typeof(PirateClientTest))]
public void NameFromString495()
{
    string s;
    s = this.NameFromString("player_name: 4420000000000000A1\u80000");
    Assert.AreEqual<string>("4420000000000000A1\u80000", s);
}
[TestMethod]
[PexGeneratedBy(typeof(PirateClientTest))]
public void NameFromString637()
{
    string s;
    s = this.NameFromString("player_name: 000");
    Assert.AreEqual<string>("000", s);
}
[TestMethod]
[PexGeneratedBy(typeof(PirateClientTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void NameFromStringThrowsContractException191()
{
    try
    {
      string s;
      s = this.NameFromString("");
      throw 
        new AssertFailedException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[TestMethod]
[PexGeneratedBy(typeof(PirateClientTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void NameFromStringThrowsContractException156()
{
    try
    {
      string s;
      s = this.NameFromString((string)null);
      throw 
        new AssertFailedException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
    }
}