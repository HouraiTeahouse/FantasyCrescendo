using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UniRx {

    public sealed class DictionaryDisposable<TKey, TValue> : IDisposable, IDictionary<TKey, TValue>
        where TValue : IDisposable {

        bool isDisposed = false;
        readonly Dictionary<TKey, TValue> inner;

        public DictionaryDisposable() { inner = new Dictionary<TKey, TValue>(); }

        public DictionaryDisposable(IEqualityComparer<TKey> comparer) {
            inner = new Dictionary<TKey, TValue>(comparer);
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys {
            get { throw new NotSupportedException("please use .Select(x => x.Key).ToArray()"); }
        }

        public Dictionary<TKey, TValue>.ValueCollection Values {
            get { throw new NotSupportedException("please use .Select(x => x.Value).ToArray()"); }
        }

        public TValue this[TKey key] {
            get {
                lock (inner) {
                    return inner[key];
                }
            }

            set {
                lock (inner) {
                    if (isDisposed)
                        value.Dispose();

                    TValue oldValue;
                    if (TryGetValue(key, out oldValue)) {
                        oldValue.Dispose();
                        inner[key] = value;
                    }
                    else {
                        inner[key] = value;
                    }
                }
            }
        }

        public int Count {
            get {
                lock (inner) {
                    return inner.Count;
                }
            }
        }

        public void Add(TKey key, TValue value) {
            lock (inner) {
                if (isDisposed) {
                    value.Dispose();
                    return;
                }

                inner.Add(key, value);
            }
        }

        public void Clear() {
            lock (inner) {
                foreach (KeyValuePair<TKey, TValue> item in inner) {
                    item.Value.Dispose();
                }
                inner.Clear();
            }
        }

        public bool Remove(TKey key) {
            lock (inner) {
                TValue oldValue;
                if (inner.TryGetValue(key, out oldValue)) {
                    bool isSuccessRemove = inner.Remove(key);
                    if (isSuccessRemove) {
                        oldValue.Dispose();
                    }
                    return isSuccessRemove;
                }
                else {
                    return false;
                }
            }
        }

        public bool ContainsKey(TKey key) {
            lock (inner) {
                return inner.ContainsKey(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value) {
            lock (inner) {
                return inner.TryGetValue(key, out value);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>) inner).IsReadOnly; }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys {
            get {
                lock (inner) {
                    return new List<TKey>(inner.Keys);
                }
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values {
            get {
                lock (inner) {
                    return new List<TValue>(inner.Values);
                }
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
            Add((TKey) item.Key, (TValue) item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
            lock (inner) {
                return ((ICollection<KeyValuePair<TKey, TValue>>) inner).Contains(item);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            lock (inner) {
                ((ICollection<KeyValuePair<TKey, TValue>>) inner).CopyTo(array, arrayIndex);
            }
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            lock (inner) {
                return
                    new List<KeyValuePair<TKey, TValue>>((ICollection<KeyValuePair<TKey, TValue>>) inner).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException();
        }

        public void Dispose() {
            lock (inner) {
                if (isDisposed)
                    return;
                isDisposed = true;

                foreach (KeyValuePair<TKey, TValue> item in inner) {
                    item.Value.Dispose();
                }
                inner.Clear();
            }
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() {
            lock (inner) {
                return new Dictionary<TKey, TValue>(inner).GetEnumerator();
            }
        }

#if !UNITY_METRO

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            lock (inner) {
                ((ISerializable) inner).GetObjectData(info, context);
            }
        }

        public void OnDeserialization(object sender) {
            lock (inner) {
                ((IDeserializationCallback) inner).OnDeserialization(sender);
            }
        }

#endif
    }

}