// <copyright file="PirateClientTest.Constructor02.g.cs">Copyright �  2011</copyright>
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
using System.Net.Sockets;
using PirateSpades.GameLogic;
using Microsoft.Pex.Framework.Generated;
using Microsoft.Pex.Engine.Exceptions;

namespace PirateSpades.Network
{
    public partial class PirateClientTest {
[TestMethod]
[PexGeneratedBy(typeof(PirateClientTest))]
public void Constructor02370()
{
    PirateClient pirateClient;
    pirateClient = this.Constructor02("4.2.2.0", "4.2.2.0", 1);
    Assert.IsNotNull((object)pirateClient);
    Assert.IsNotNull(pirateClient.Socket);
    Assert.AreEqual<bool>(true, pirateClient.Socket.Blocking);
    Assert.AreEqual<bool>(false, pirateClient.Socket.UseOnlyOverlappedIO);
    Assert.AreEqual<AddressFamily>
        (AddressFamily.InterNetwork, pirateClient.Socket.AddressFamily);
    Assert.AreEqual<SocketType>(SocketType.Stream, pirateClient.Socket.SocketType);
    Assert.AreEqual<ProtocolType>
        (ProtocolType.Tcp, pirateClient.Socket.ProtocolType);
    Assert.AreEqual<bool>(false, pirateClient.Socket.IsBound);
    Assert.AreEqual<int>(4096, pirateClient.BufferSize);
    Assert.AreEqual<bool>(false, pirateClient.DebugMode);
    Assert.AreEqual<bool>(false, pirateClient.VirtualPlayer);
    Assert.AreEqual<bool>(true, pirateClient.IsDead);
    Assert.IsNull(((Player)pirateClient).Game);
    Assert.AreEqual<string>("4.2.2.0", ((Player)pirateClient).Name);
    Assert.AreEqual<int>(0, ((Player)pirateClient).Bet);
    Assert.AreEqual<bool>(false, ((Player)pirateClient).IsDealer);
    Assert.IsNull(((Player)pirateClient).CardToPlay);
    Assert.IsNotNull(((Player)pirateClient).Hand);
}
[TestMethod]
[PexGeneratedBy(typeof(PirateClientTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void Constructor02ThrowsContractException338()
{
    try
    {
      PirateClient pirateClient;
      pirateClient = this.Constructor02("", "", 16777217);
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
public void Constructor02ThrowsContractException388()
{
    try
    {
      PirateClient pirateClient;
      pirateClient = this.Constructor02("", "", 1);
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
public void Constructor02ThrowsContractException498()
{
    try
    {
      PirateClient pirateClient;
      pirateClient = this.Constructor02("", "", 0);
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
public void Constructor02ThrowsContractException386()
{
    try
    {
      PirateClient pirateClient;
      pirateClient = this.Constructor02("", (string)null, 0);
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
public void Constructor02ThrowsContractException677()
{
    try
    {
      PirateClient pirateClient;
      pirateClient = this.Constructor02((string)null, (string)null, 0);
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