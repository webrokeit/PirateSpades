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

    /// <summary>
    /// A dictionarty that keeps track of the insertion order
    /// Make a new dictionary with this type as key and this type as value
    /// </summary>
    /// <typeparam name="TKey">The key</typeparam>
    /// <typeparam name="TValue">The value</typeparam>
    public class OrderedDictionary<TKey, TValue> : System.Collections.Specialized.OrderedDictionary {
        /// <summary>
        /// Get a collection containing all the keys
        /// </summary>
        public new ICollection<TKey> Keys {
            get {
                return base.Keys.Cast<TKey>().ToList();
            }
        } 

        /// <summary>
        /// Get a collection containing all the values
        /// </summary>
        public new ICollection<TValue> Values {
            get {
                Contract.Requires(base.Count > 0);
                return base.Values.Cast<TValue>().ToList();
            }
        }

        /// <summary>
        /// Set the value at this key
        /// Get the value at this key
        /// </summary>
        /// <param name="key">The value to be inputted</param>
        /// <returns>The value at this key</returns>
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

        /// <summary>
        /// Set the value at this key
        /// Get the value at this key
        /// </summary>
        /// <param name="key">The value to be inputted</param>
        /// <returns>The value at this key</returns>
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

        /// <summary>
        /// Add this value at this index
        /// Get the value at this index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The value at the index</returns>
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

        /// <summary>
        /// Add this key and value
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public new void Add(object key, object value) {
            Contract.Requires(key != null && key is TKey && value != null && value is TValue && !this.ContainsKey((TKey)key));
            this.Add((TKey)key, (TValue)value);
        }

        /// <summary>
        /// Add this key and value
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void Add(TKey key, TValue value) {
            Contract.Requires(!ReferenceEquals(key, null) && !this.ContainsKey(key) && !ReferenceEquals(value, null));
            base.Add(key, value);
        }

        /// <summary>
        /// Remove this key
        /// </summary>
        /// <param name="key">The key to remove</param>
        public new void Remove(object key) {
            Contract.Requires(key != null && key is TKey);
            this.Remove((TKey) key);
        }

        /// <summary>
        /// Remove this key
        /// </summary>
        /// <param name="key">The key to remove</param>
        public void Remove(TKey key) {
            Contract.Requires(!ReferenceEquals(key, null));
            base.Remove(key);
        }

        /// <summary>
        /// Remove the item at the given index
        /// </summary>
        /// <param name="index">The index</param>
        public new void RemoveAt(int index) {
            Contract.Requires(index >= 0 && index < base.Count);
            base.RemoveAt(index);
        }

        /// <summary>
        /// Insert this key and value at the given index
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public new void Insert(int index, object key, object value) {
            Contract.Requires(index >= 0 && index < base.Count && key is TKey && value is TValue);
            Contract.Requires(key != null && !ContainsKey((TKey)key) && value != null);
            this.Insert(index, (TKey)key, (TValue)value);
        }

        /// <summary>
        /// Insert this key and this value at this index
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void Insert(int index, TKey key, TValue value) {
            Contract.Requires(index >= 0 && index < base.Count);
            Contract.Requires(!ReferenceEquals(key, null) && !ContainsKey(key) && !ReferenceEquals(value, null));
            base.Insert(index, key, value);
        }

        /// <summary>
        /// Does the dictionary contain this key?
        /// </summary>
        /// <param name="obj">The key to check for</param>
        /// <returns>True if the dictionary contains the key</returns>
        [Pure]
        public new bool Contains(Object obj) {
            Contract.Requires(obj != null && obj is TKey);
            return this.ContainsKey((TKey)obj);
        }

        /// <summary>
        /// Does the dictionary contain this key?
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>True if the dictionary contains the key</returns>
        [Pure]
        public bool ContainsKey(TKey key) {
            Contract.Requires(!ReferenceEquals(key, null));
            return base.Contains(key);
        }

        /// <summary>
        /// Does the dictionary contain this value?
        /// </summary>
        /// <param name="value">The value to check for</param>
        /// <returns>True if the dictionary contains the value</returns>
        [Pure]
        public bool ContainsValue(TValue value) {
            Contract.Requires(!ReferenceEquals(value, null));
            return this.Values.Cast<TValue>().Contains(value);
        }

        /// <summary>
        /// Make a immutable ordered dictionary with this type as key and this type as value
        /// </summary>
        /// <returns>The immutable ordered dictionary</returns>
        public ImmutableOrderedDictionary<TKey, TValue> AsImmutable() {
            return new ImmutableOrderedDictionary<TKey, TValue>(this);
        } 
    }
}
