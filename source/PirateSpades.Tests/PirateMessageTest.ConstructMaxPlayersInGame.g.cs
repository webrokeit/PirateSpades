// <copyright file="PirateMessageTest.ConstructMaxPlayersInGame.g.cs">Copyright �  2011</copyright>
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
    public partial class PirateMessageTest {
[TestMethod]
[PexGeneratedBy(typeof(PirateMessageTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void ConstructMaxPlayersInGameThrowsContractException674()
{
    try
    {
      string s;
      s = this.ConstructMaxPlayersInGame(int.MinValue);
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
[PexGeneratedBy(typeof(PirateMessageTest))]
public void ConstructMaxPlayersInGame909()
{
    string s;
    s = this.ConstructMaxPlayersInGame(1);
    Assert.AreEqual<string>("players_ingamemax: 1", s);
}
[TestMethod]
[PexGeneratedBy(typeof(PirateMessageTest))]
public void ConstructMaxPlayersInGame76()
{
    string s;
    s = this.ConstructMaxPlayersInGame(0);
    Assert.AreEqual<string>("players_ingamemax: 0", s);
}
    }
}
