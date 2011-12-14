// <copyright file="PirateBroadcasterTest.Constructor01.g.cs">Copyright �  2011</copyright>
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
    public partial class PirateBroadcasterTest {
[TestMethod]
[PexGeneratedBy(typeof(PirateBroadcasterTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void Constructor01ThrowsContractException241()
{
    try
    {
      PirateBroadcaster pirateBroadcaster;
      byte[] bs = new byte[0];
      pirateBroadcaster = this.Constructor01(bs, 65537, 0);
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
[PexGeneratedBy(typeof(PirateBroadcasterTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void Constructor01ThrowsContractException417()
{
    try
    {
      PirateBroadcaster pirateBroadcaster;
      byte[] bs = new byte[0];
      pirateBroadcaster = this.Constructor01(bs, int.MinValue, 0);
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
[PexGeneratedBy(typeof(PirateBroadcasterTest))]
public void Constructor01866()
{
    PirateBroadcaster pirateBroadcaster;
    byte[] bs = new byte[0];
    pirateBroadcaster = this.Constructor01(bs, 0, 1);
    Assert.IsNotNull((object)pirateBroadcaster);
    Assert.IsNotNull(pirateBroadcaster.Message);
    Assert.AreEqual<int>(0, pirateBroadcaster.Message.Length);
    Assert.AreEqual<int>(0, pirateBroadcaster.Port);
}
[TestMethod]
[PexGeneratedBy(typeof(PirateBroadcasterTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void Constructor01ThrowsContractException906()
{
    try
    {
      PirateBroadcaster pirateBroadcaster;
      byte[] bs = new byte[0];
      pirateBroadcaster = this.Constructor01(bs, 0, 0);
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
[PexGeneratedBy(typeof(PirateBroadcasterTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void Constructor01ThrowsContractException88()
{
    try
    {
      PirateBroadcaster pirateBroadcaster;
      pirateBroadcaster = this.Constructor01((byte[])null, 0, 0);
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
[PexGeneratedBy(typeof(PirateBroadcasterTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void ConstructorThrowsContractException564()
{
    try
    {
      PirateBroadcaster pirateBroadcaster;
      pirateBroadcaster = this.Constructor(65537, 0);
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
[PexGeneratedBy(typeof(PirateBroadcasterTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void ConstructorThrowsContractException902()
{
    try
    {
      PirateBroadcaster pirateBroadcaster;
      pirateBroadcaster = this.Constructor(int.MinValue, 0);
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
[PexGeneratedBy(typeof(PirateBroadcasterTest))]
public void Constructor266()
{
    PirateBroadcaster pirateBroadcaster;
    pirateBroadcaster = this.Constructor(0, 1);
    Assert.IsNotNull((object)pirateBroadcaster);
    Assert.IsNotNull(pirateBroadcaster.Message);
    Assert.AreEqual<int>(0, pirateBroadcaster.Message.Length);
    Assert.AreEqual<int>(0, pirateBroadcaster.Port);
}
[TestMethod]
[PexGeneratedBy(typeof(PirateBroadcasterTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void ConstructorThrowsContractException848()
{
    try
    {
      PirateBroadcaster pirateBroadcaster;
      pirateBroadcaster = this.Constructor(0, 0);
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