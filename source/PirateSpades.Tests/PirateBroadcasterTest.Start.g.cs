// <copyright file="PirateBroadcasterTest.Start.g.cs">Copyright �  2011</copyright>
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
public void StartThrowsContractException681()
{
    try
    {
      PirateBroadcaster pirateBroadcaster;
      pirateBroadcaster = new PirateBroadcaster(0, 1);
      pirateBroadcaster.Start();
      this.Start(pirateBroadcaster);
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