// <copyright file="ImmutableOrderedDictionaryTKeyTValueTest.Remove01.g.cs">Copyright �  2011</copyright>
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

namespace PirateSpades.Misc
{
    public partial class ImmutableOrderedDictionaryTKeyTValueTest {
[TestMethod]
[PexGeneratedBy(typeof(ImmutableOrderedDictionaryTKeyTValueTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void Remove01ThrowsContractException537()
{
    try
    {
      OrderedDictionary<int, int> orderedDictionary;
      ImmutableOrderedDictionary<int, int> immutableOrderedDictionary;
      object boxi = (object)(default(int));
      object boxi1 = (object)(default(int));
      orderedDictionary = new OrderedDictionary<int, int>();
      orderedDictionary[boxi] = boxi1;
      immutableOrderedDictionary =
        new ImmutableOrderedDictionary<int, int>(orderedDictionary);
      this.Remove01<int, int>(immutableOrderedDictionary, 0);
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
[PexGeneratedBy(typeof(ImmutableOrderedDictionaryTKeyTValueTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void RemoveThrowsContractException541()
{
    try
    {
      OrderedDictionary<int, int> orderedDictionary;
      ImmutableOrderedDictionary<int, int> immutableOrderedDictionary;
      object boxi = (object)(default(int));
      object boxi1 = (object)(default(int));
      orderedDictionary = new OrderedDictionary<int, int>();
      orderedDictionary[boxi] = boxi1;
      immutableOrderedDictionary =
        new ImmutableOrderedDictionary<int, int>(orderedDictionary);
      object s0 = new object();
      this.Remove<int, int>(immutableOrderedDictionary, s0);
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
[PexGeneratedBy(typeof(ImmutableOrderedDictionaryTKeyTValueTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void RemoveThrowsContractException362()
{
    try
    {
      OrderedDictionary<int, int> orderedDictionary;
      ImmutableOrderedDictionary<int, int> immutableOrderedDictionary;
      object boxi = (object)(default(int));
      object boxi1 = (object)(default(int));
      orderedDictionary = new OrderedDictionary<int, int>();
      orderedDictionary[boxi] = boxi1;
      immutableOrderedDictionary =
        new ImmutableOrderedDictionary<int, int>(orderedDictionary);
      this.Remove<int, int>(immutableOrderedDictionary, (object)null);
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