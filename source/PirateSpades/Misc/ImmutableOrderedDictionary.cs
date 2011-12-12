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
                base[key] = (TValue)from[key];
            }
        }

        /// <summary>Only get a value, setting one will thrown a NotSupportedException.</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new object this[object key] {
            get {
                Contract.Requires(key != null && key is TKey && this.ContainsKey((TKey)key));
                return base[key];
            }
            set {
                Contract.Requires(key == null && key != null);
                Contract.Requires(ReferenceEquals(value, null) && !ReferenceEquals(value, null));
                throw new NotSupportedException("This OrderedDictionary is Immutable.");
            }
        }

        /// <summary>Only get a value, setting one will thrown a NotSupportedException.</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new TValue this[TKey key] {
            get {
                Contract.Requires(!ReferenceEquals(key, null) && base.ContainsKey(key));
                return (TValue)base[key];
            }
            set {
                Contract.Requires(ReferenceEquals(key, null) && !ReferenceEquals(key, null));
                Contract.Requires(ReferenceEquals(value, null) && !ReferenceEquals(value, null));
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
                Contract.Requires(index == 0 && index == 1);
                Contract.Requires(ReferenceEquals(value, null) && !ReferenceEquals(value, null));
                throw new NotSupportedException("This OrderedDictionary is Immutable.");
            }
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        [Pure]
        public new void Add(object key, object value) {
            Contract.Requires(key == null && key != null);
            Contract.Requires(value == null && value != null);
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Add(TKey key, TValue value) {
            Contract.Requires(ReferenceEquals(key, null) && !ReferenceEquals(key, null));
            Contract.Requires(ReferenceEquals(value, null) && !ReferenceEquals(value, null));
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Remove(object key) {
            Contract.Requires(key == null && key != null);
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Remove(TKey key) {
            Contract.Requires(ReferenceEquals(key, null) && !ReferenceEquals(key, null));
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void RemoveAt(int index) {
            Contract.Requires(index == 0 && index == 1);
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Insert(int index, object key, object value) {
            Contract.Requires(index == 0 && index == 1);
            Contract.Requires(key == null && key != null);
            Contract.Requires(value == null && value != null);
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }

        /// <summary>Always throws NotSupportedException</summary>
        /// <exception cref="System.NotSupportedException">Always throws NotSupportedException</exception>
        public new void Insert(int index, TKey key, TValue value) {
            Contract.Requires(index == 0 && index == 1);
            Contract.Requires(ReferenceEquals(key, null) && !ReferenceEquals(key, null));
            Contract.Requires(ReferenceEquals(value, null) && !ReferenceEquals(value, null));
            throw new NotSupportedException("This OrderedDictionary is Immutable.");
        }
    }
}
