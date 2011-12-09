// <copyright file="ImmutableOrderedDictionaryTKeyTValueTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Misc;

namespace PirateSpades.Misc
{
    [TestClass]
    [PexClass(typeof(ImmutableOrderedDictionary<, >))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ImmutableOrderedDictionaryTKeyTValueTest
    {
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void RemoveAt<TKey, TValue>([PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target, int index) {
            target.RemoveAt(index);
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.RemoveAt(ImmutableOrderedDictionary`2<!!0,!!1>, Int32)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Remove01<TKey, TValue>([PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target, TKey key) {
            target.Remove(key);
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.Remove01(ImmutableOrderedDictionary`2<!!0,!!1>, !!0)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Remove<TKey, TValue>([PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target, object key) {
            target.Remove(key);
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.Remove(ImmutableOrderedDictionary`2<!!0,!!1>, Object)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void ItemSet02<TKey, TValue>(
            [PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target,
            int index,
            TValue value
        ) {
            target[index] = value;
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.ItemSet02(ImmutableOrderedDictionary`2<!!0,!!1>, Int32, !!1)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void ItemSet01<TKey, TValue>(
            [PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target,
            TKey key,
            TValue value
        ) {
            target[key] = value;
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.ItemSet01(ImmutableOrderedDictionary`2<!!0,!!1>, !!0, !!1)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void ItemSet<TKey, TValue>(
            [PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target,
            object key,
            object value
        ) {
            target[key] = value;
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.ItemSet(ImmutableOrderedDictionary`2<!!0,!!1>, Object, Object)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public TValue ItemGet02<TKey, TValue>([PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target, int index) {
            TValue result = target[index];
            return result;
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.ItemGet02(ImmutableOrderedDictionary`2<!!0,!!1>, Int32)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public TValue ItemGet01<TKey, TValue>([PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target, TKey key) {
            TValue result = target[key];
            return result;
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.ItemGet01(ImmutableOrderedDictionary`2<!!0,!!1>, !!0)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public object ItemGet<TKey, TValue>([PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target, object key) {
            object result = target[key];
            return result;
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.ItemGet(ImmutableOrderedDictionary`2<!!0,!!1>, Object)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Insert01<TKey, TValue>(
            [PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target,
            int index,
            TKey key,
            TValue value
        ) {
            target.Insert(index, key, value);
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.Insert01(ImmutableOrderedDictionary`2<!!0,!!1>, Int32, !!0, !!1)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Insert<TKey, TValue>(
            [PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target,
            int index,
            object key,
            object value
        ) {
            target.Insert(index, key, value);
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.Insert(ImmutableOrderedDictionary`2<!!0,!!1>, Int32, Object, Object)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public ImmutableOrderedDictionary<TKey, TValue> Constructor<TKey, TValue>(OrderedDictionary<TKey, TValue> from) {
            ImmutableOrderedDictionary<TKey, TValue> target = new ImmutableOrderedDictionary<TKey, TValue>(from)
              ;
            return target;
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.Constructor(OrderedDictionary`2<!!0,!!1>)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Add01<TKey, TValue>(
            [PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target,
            TKey key,
            TValue value
        ) {
            target.Add(key, value);
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.Add01(ImmutableOrderedDictionary`2<!!0,!!1>, !!0, !!1)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Add<TKey, TValue>(
            [PexAssumeUnderTest]ImmutableOrderedDictionary<TKey, TValue> target,
            object key,
            object value
        ) {
            target.Add(key, value);
            // TODO: add assertions to method ImmutableOrderedDictionaryTKeyTValueTest.Add(ImmutableOrderedDictionary`2<!!0,!!1>, Object, Object)
        }
    }
}
