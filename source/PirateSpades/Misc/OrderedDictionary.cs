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
                return base.Values.Cast<TValue>().ToList();
            }
        }

        public new object this[object key] {
            get {
                Contract.Requires(key is TKey);
                return base[key];
            }
            set {
                Contract.Requires(key is TKey);
                base[key] = value;
            }
        }

        public TValue this[TKey key] {
            get {
                return (TValue)base[key];
            }
            set {
                base[key] = value;
            }
        }

        public new TValue this[int index] {
            get {
                Contract.Requires(index >= 0 && index < base.Count);
                return (TValue)base[index];
            }
            set {
                Contract.Requires(index >= 0 && index < base.Count);
                base[index] = value;
            }
        }

        public new void Add(object key, object value) {
            Contract.Requires(key is TKey && value is TValue);
            this.Add((TKey)key, (TValue)value);
        }

        public void Add(TKey key, TValue value) {
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
            this.Insert(index, (TKey)key, (TValue)value);
        }

        public void Insert(int index, TKey key, TValue value) {
            Contract.Requires(index >= 0 && index < base.Count);
            base.Insert(index, key, value);
        }

        [Pure]
        public new bool Contains(Object obj) {
            Contract.Requires(obj is TKey);
            return this.ContainsKey((TKey)obj);
        }

        [Pure]
        public bool ContainsKey(TKey key) {
            return base.Contains(key);
        }

        [Pure]
        public bool ContainsValue(TValue value) {
            return this.Values.Cast<TValue>().Contains(value);
        }
    }
}
