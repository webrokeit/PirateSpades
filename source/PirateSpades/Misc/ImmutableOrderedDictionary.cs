using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.Misc {
    using System.Diagnostics.Contracts;

    public class ImmutableOrderedDictionary<TKey, TValue> : OrderedDictionary<TKey, TValue> {

        public ImmutableOrderedDictionary(OrderedDictionary<TKey, TValue> from) {
            Contract.Requires(from != null);
            foreach(var key in from.Keys) {
                base[key] = from[key];
            }
        }

        /// <summary>Only get a value, setting one will thrown a NotSupportedException.</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new object this[object key] {
            get {
                Contract.Requires(key is TKey);
                return base[key];
            }
            set {
                throw new NotSupportedException("This OrderedDictionary is Immutable.");
            }
        }

        /// <summary>Only get a value, setting one will thrown a NotSupportedException.</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new TValue this[TKey key] {
            get {
                return (TValue)base[key];
            }
            set {
                throw new NotSupportedException("This OrderedDictionary is Immutable.");
            }
        }

        /// <summary>Only get a value, setting one will thrown a NotSupportedException.</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new TValue this[int index] {
            get {
                Contract.Requires(index >= 0 && index < base.Count);
                return (TValue)base[index];
            }
            set {
                throw new NotSupportedException("This OrderedDictionary is Immutable.");
            }
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Add(object key, object value) {
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Add(TKey key, TValue value) {
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Remove(object key) {
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Remove(TKey key) {
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void RemoveAt(int index) {
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Insert(int index, object key, object value) {
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Insert(int index, TKey key, TValue value) {
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }
    }
}
