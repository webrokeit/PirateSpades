// <copyright file="PirateHostTest.Constructor.g.cs">Copyright �  2011</copyright>
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
    public partial class PirateHostTest {
[TestMethod]
[PexGeneratedBy(typeof(PirateHostTest))]
public void Constructor922()
{
    PirateHost pirateHost;
    pirateHost = this.Constructor(1);
    Assert.IsNotNull((object)pirateHost);
    Assert.AreEqual<bool>(false, pirateHost.Started);
    Assert.AreEqual<bool>(false, pirateHost.AcceptNewConnections);
    Assert.AreEqual<bool>(false, pirateHost.DebugMode);
    Assert.IsNull(pirateHost.Game);
    Assert.IsNotNull(pirateHost.Broadcaster);
    Assert.IsNotNull(pirateHost.Broadcaster.Message);
    Assert.AreEqual<int>(21, pirateHost.Broadcaster.Message.Length);
    Assert.AreEqual<byte>((byte)48, pirateHost.Broadcaster.Message[0]);
    Assert.AreEqual<byte>((byte)48, pirateHost.Broadcaster.Message[1]);
    Assert.AreEqual<byte>((byte)49, pirateHost.Broadcaster.Message[2]);
    Assert.AreEqual<byte>((byte)55, pirateHost.Broadcaster.Message[3]);
    Assert.AreEqual<byte>((byte)66, pirateHost.Broadcaster.Message[4]);
    Assert.AreEqual<byte>((byte)67, pirateHost.Broadcaster.Message[5]);
    Assert.AreEqual<byte>((byte)83, pirateHost.Broadcaster.Message[6]);
    Assert.AreEqual<byte>((byte)84, pirateHost.Broadcaster.Message[7]);
    Assert.AreEqual<byte>((byte)49, pirateHost.Broadcaster.Message[8]);
    Assert.AreEqual<byte>((byte)57, pirateHost.Broadcaster.Message[9]);
    Assert.AreEqual<byte>((byte)50, pirateHost.Broadcaster.Message[10]);
    Assert.AreEqual<byte>((byte)46, pirateHost.Broadcaster.Message[11]);
    Assert.AreEqual<byte>((byte)49, pirateHost.Broadcaster.Message[12]);
    Assert.AreEqual<byte>((byte)54, pirateHost.Broadcaster.Message[13]);
    Assert.AreEqual<byte>((byte)56, pirateHost.Broadcaster.Message[14]);
    Assert.AreEqual<byte>((byte)46, pirateHost.Broadcaster.Message[15]);
    Assert.AreEqual<byte>((byte)49, pirateHost.Broadcaster.Message[16]);
    Assert.AreEqual<byte>((byte)46, pirateHost.Broadcaster.Message[17]);
    Assert.AreEqual<byte>((byte)49, pirateHost.Broadcaster.Message[18]);
    Assert.AreEqual<byte>((byte)48, pirateHost.Broadcaster.Message[19]);
    Assert.AreEqual<byte>((byte)49, pirateHost.Broadcaster.Message[20]);
    Assert.AreEqual<int>(1, pirateHost.Broadcaster.Port);
}
[TestMethod]
[PexGeneratedBy(typeof(PirateHostTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void ConstructorThrowsContractException164()
{
    try
    {
      PirateHost pirateHost;
      pirateHost = this.Constructor(0);
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