// <copyright file="OrderedDictionaryTKeyTValueTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Misc;
using System.Collections.Generic;

namespace PirateSpades.Misc
{
    [TestClass]
    [PexClass(typeof(OrderedDictionary<, >))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class OrderedDictionaryTKeyTValueTest
    {
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public ICollection<TValue> ValuesGet<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target) {
            ICollection<TValue> result = target.Values;
            return result;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.ValuesGet(OrderedDictionary`2<!!0,!!1>)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void RemoveAt<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target, int index) {
            target.RemoveAt(index);
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.RemoveAt(OrderedDictionary`2<!!0,!!1>, Int32)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Remove01<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target, TKey key) {
            target.Remove(key);
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.Remove01(OrderedDictionary`2<!!0,!!1>, !!0)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Remove<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target, object key) {
            target.Remove(key);
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.Remove(OrderedDictionary`2<!!0,!!1>, Object)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public ICollection<TKey> KeysGet<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target) {
            ICollection<TKey> result = target.Keys;
            return result;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.KeysGet(OrderedDictionary`2<!!0,!!1>)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void ItemSet02<TKey, TValue>(
            [PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target,
            int index,
            TValue value
        ) {
            target[index] = value;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.ItemSet02(OrderedDictionary`2<!!0,!!1>, Int32, !!1)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void ItemSet01<TKey, TValue>(
            [PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target,
            TKey key,
            TValue value
        ) {
            target[key] = value;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.ItemSet01(OrderedDictionary`2<!!0,!!1>, !!0, !!1)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void ItemSet<TKey, TValue>(
            [PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target,
            object key,
            object value
        ) {
            target[key] = value;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.ItemSet(OrderedDictionary`2<!!0,!!1>, Object, Object)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public TValue ItemGet02<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target, int index) {
            TValue result = target[index];
            return result;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.ItemGet02(OrderedDictionary`2<!!0,!!1>, Int32)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public TValue ItemGet01<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target, TKey key) {
            TValue result = target[key];
            return result;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.ItemGet01(OrderedDictionary`2<!!0,!!1>, !!0)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public object ItemGet<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target, object key) {
            object result = target[key];
            return result;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.ItemGet(OrderedDictionary`2<!!0,!!1>, Object)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Insert01<TKey, TValue>(
            [PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target,
            int index,
            TKey key,
            TValue value
        ) {
            target.Insert(index, key, value);
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.Insert01(OrderedDictionary`2<!!0,!!1>, Int32, !!0, !!1)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Insert<TKey, TValue>(
            [PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target,
            int index,
            object key,
            object value
        ) {
            target.Insert(index, key, value);
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.Insert(OrderedDictionary`2<!!0,!!1>, Int32, Object, Object)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public bool ContainsValue<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target, TValue value) {
            bool result = target.ContainsValue(value);
            return result;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.ContainsValue(OrderedDictionary`2<!!0,!!1>, !!1)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public bool ContainsKey<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target, TKey key) {
            bool result = target.ContainsKey(key);
            return result;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.ContainsKey(OrderedDictionary`2<!!0,!!1>, !!0)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public bool Contains<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target, object obj) {
            bool result = target.Contains(obj);
            return result;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.Contains(OrderedDictionary`2<!!0,!!1>, Object)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public OrderedDictionary<TKey, TValue> Constructor<TKey, TValue>() {
            OrderedDictionary<TKey, TValue> target = new OrderedDictionary<TKey, TValue>();
            return target;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.Constructor()
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public ImmutableOrderedDictionary<TKey, TValue> AsImmutable<TKey, TValue>([PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target) {
            ImmutableOrderedDictionary<TKey, TValue> result = target.AsImmutable();
            return result;
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.AsImmutable(OrderedDictionary`2<!!0,!!1>)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Add01<TKey, TValue>(
            [PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target,
            TKey key,
            TValue value
        ) {
            target.Add(key, value);
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.Add01(OrderedDictionary`2<!!0,!!1>, !!0, !!1)
        }
        [PexGenericArguments(typeof(int), typeof(int))]
        [PexMethod]
        public void Add<TKey, TValue>(
            [PexAssumeUnderTest]OrderedDictionary<TKey, TValue> target,
            object key,
            object value
        ) {
            target.Add(key, value);
            // TODO: add assertions to method OrderedDictionaryTKeyTValueTest.Add(OrderedDictionary`2<!!0,!!1>, Object, Object)
        }
    }
}
