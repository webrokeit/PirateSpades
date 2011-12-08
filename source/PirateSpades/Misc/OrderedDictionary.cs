// <copyright file="OrderedDictionary.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A generic version of the OrderedDictionary.
//      It's more or less just a proxy for the OrderedDictionary found in System.Collections.Specialized.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Misc {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics.Contracts;

    public class OrderedDictionary<TKey, TValue> : System.Collections.Specialized.OrderedDictionary {
        public new ICollection<TKey> Keys {
            get {
                return base.Keys.Cast<TKey>().ToList();
            }
        } 

        public new ICollection<TValue> Values {
            get {
                Contract.Requires(base.Count > 0);
                return base.Values.Cast<TValue>().ToList();
            }
        }

        public new object this[object key] {
            get {
                Contract.Requires(key != null && key is TKey && this.ContainsKey((TKey)key));
                var value = base[key];
                return !ReferenceEquals(value, null) ? value : null;
            }
            set {
                Contract.Requires(key is TKey && !ReferenceEquals(key, null) && value is TValue && !ReferenceEquals(value, null));
                base[key] = value;
            }
        }

        public TValue this[TKey key] {
            get {
                Contract.Requires(!ReferenceEquals(key, null) && this.ContainsKey(key));
                return (TValue)base[key];
            }
            set {
                Contract.Requires(!ReferenceEquals(key, null) && !ReferenceEquals(value, null));
                base[key] = value;
            }
        }

        public new TValue this[int index] {
            get {
                Contract.Requires(index >= 0 && index < base.Count);
                return (TValue)base[index];
            }
            set {
                Contract.Requires(index >= 0 && index < base.Count && !ReferenceEquals(value, null));
                base[index] = value;
            }
        }

        public new void Add(object key, object value) {
            Contract.Requires(key != null && key is TKey && value != null && value is TValue && !this.ContainsKey((TKey)key));
            this.Add((TKey)key, (TValue)value);
        }

        public void Add(TKey key, TValue value) {
            Contract.Requires(!ReferenceEquals(key, null) && !this.ContainsKey(key) && !ReferenceEquals(value, null));
            base.Add(key, value);
        }

        public new void Remove(object key) {
            Contract.Requires(key is TKey);
            this.Remove((TKey) key);
        }

        public void Remove(TKey key) {
            base.Remove(key);
        }

        public new void RemoveAt(int index) {
            Contract.Requires(index >= 0 && index < base.Count);
            base.RemoveAt(index);
        }

        public new void Insert(int index, object key, object value) {
            Contract.Requires(index >= 0 && index < base.Count && key is TKey && value is TValue);
            Contract.Requires(key != null && !ContainsKey((TKey)key) && value != null);
            this.Insert(index, (TKey)key, (TValue)value);
        }

        public void Insert(int index, TKey key, TValue value) {
            Contract.Requires(index >= 0 && index < base.Count);
            Contract.Requires(!ReferenceEquals(key, null) && !ContainsKey(key) && !ReferenceEquals(value, null));
            base.Insert(index, key, value);
        }

        [Pure]
        public new bool Contains(Object obj) {
            Contract.Requires(obj != null && obj is TKey);
            return this.ContainsKey((TKey)obj);
        }

        [Pure]
        public bool ContainsKey(TKey key) {
            Contract.Requires(!ReferenceEquals(key, null));
            return base.Contains(key);
        }

        [Pure]
        public bool ContainsValue(TValue value) {
            Contract.Requires(!ReferenceEquals(value, null));
            return this.Values.Cast<TValue>().Contains(value);
        }

        public ImmutableOrderedDictionary<TKey, TValue> AsImmutable() {
            return new ImmutableOrderedDictionary<TKey, TValue>(this);
        } 
    }
}
