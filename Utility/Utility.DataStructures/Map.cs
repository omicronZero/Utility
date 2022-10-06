using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Utility.Collections;
using Utility.Collections.Tools;

namespace Utility.DataStructures
{
    //TODO: complete documentation
    /// <summary>
    /// Provides an efficient map of keys to corresponding values.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <remarks>
    /// Acts as a dictionary.
    /// For distinct key hash codes has a guaranteed performance in O(1) for all operations if the expansion threshold is set to 1 if the internally used buckets have
    /// been initialized to a value (otherwise, the running time time will be in O(resolution)).
    /// For identical key hash codes, the structure will use array lists as a fallback.
    /// It is recommended that you use an implementation of <see cref="IEqualityComparer{T}"/> that arranges keys to similar but distinct hash codes where
    /// the latter is of higher importance regarding execution time. Hashcodes are not remapped to a different location in the map.
    /// </remarks>
    public class Map<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly int _hashCodeSegmentSize;
        private readonly int _expansionThreshold;
        private readonly int _collapseThreshold;
        private Bucket _root;
        private readonly ItemComparer _itemComparer;

        public IEqualityComparer<TKey> KeyComparer { get; }

        //TODO: complete documentation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution">The resolution of each bucket. It should depend on the number of items being stored.
        /// See remarks of <see cref="Map{TKey, TValue}"/> for further information.</param>
        /// <param name="keyComparer"></param>
        public Map(int resolution, IEqualityComparer<TKey> keyComparer, int expansionThreshold, int collapseThreshold)
        {
            if (resolution <= 0 || resolution > 32)
                throw new ArgumentException("Resolution between 0 and 32 expected.", nameof(resolution));

            if (expansionThreshold < 0)
                throw new ArgumentOutOfRangeException(nameof(expansionThreshold), "Non-negative expansion threshold expected.");
            if (collapseThreshold < 0)
                throw new ArgumentOutOfRangeException(nameof(collapseThreshold), "Non-negative collapse threshold expected.");

            if (collapseThreshold >= expansionThreshold)
                throw new ArgumentException("Collapse threshold must be less than expansion threshold.");

            _hashCodeSegmentSize = resolution;
            KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _itemComparer = new ItemComparer(KeyComparer);
            _root = new Bucket(0, resolution, this);
            _expansionThreshold = expansionThreshold;
            _collapseThreshold = collapseThreshold;
        }

        protected Entry GetItem(TKey key) => new Entry(KeyComparer.GetHashCode(key), key, default);
        protected Entry GetItem(TKey key, TValue value)
        {
            return new Entry(GetItem(key), value);
        }

        protected Entry GetItem(KeyValuePair<TKey, TValue> pair) => GetItem(pair.Key, pair.Value);

        public ICollection<TKey> Keys => new CollectionAccessor<TKey>(this.Select((s) => s.Key), () => Count, ContainsKey);

        public ICollection<TValue> Values => new CollectionAccessor<TValue>(this.Select((s) => s.Value), () => Count);

        public int Count => _root.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public TValue this[TKey key]
        {
            get
            {
                TValue v;

                if (!TryGetValue(key, out v))
                    throw new KeyNotFoundException();

                return v;
            }
            set
            {
                _root.Insert(GetItem(key, value));
            }
        }

        protected virtual MapLeafCollection CreateLeafCollection(ItemComparer itemComparer)
        {
            return new ListMapLeafCollection(itemComparer);
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// See remarks for notes about the running time.
        /// </summary>
        /// <param name="key">The key that is used to access the specified value.</param>
        /// <param name="value">The value corresponding to the specified key.</param>
        /// <exception cref="ArgumentException">The specified key is already in the dictionary.</exception>
        /// <remarks>
        /// For non-colliding key hash-codes, this method runs in O(1).
        /// 
        /// For colliding key hash-codes, the running time will be bounded by the number of colliding items.
        /// If m is the number of items with the same hash code, the running time will be in O(m).
        /// A more efficient way of dealing with such collisions is to derive a type from <see cref="Map{TKey, TValue}"/>
        /// and add an override to <see cref="CreateLeafCollection(ItemComparer)"/> if possible as the standard approach
        /// uses a list.
        /// </remarks>
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new ArgumentException("The specified key is already in the dictionary.");
        }

        public bool ContainsKey(TKey key)
        {
            return _root.Find(GetItem(key), out _);
        }

        public bool Remove(TKey key)
        {
            return _root.Remove(GetItem(key));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Entry item;

            if (!_root.Find(GetItem(key), out item))
            {
                value = default;
                return false;
            }

            value = item.Value;
            return true;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return _root.Insert(GetItem(key, value));
        }

        public bool TryAdd(KeyValuePair<TKey, TValue> value)
        {
            return TryAdd(value.Key, value.Value);
        }

        public void Clear()
        {
            _root = new Bucket(0, _hashCodeSegmentSize, this);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue v;

            return TryGetValue(item.Key, out v) && EqualityComparer<TValue>.Default.Equals(v, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);

            foreach (var kvp in this)
                array[arrayIndex++] = kvp;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _root.Remove(GetItem(item));
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return GetAllLeafBuckets().SelectMany((s) => s.Entries).Select((s) => s.GetPair()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<Bucket> GetAllLeafBuckets()
        {
            Stack<Bucket> buckets = new Stack<Bucket>();

            buckets.Push(_root);

            do
            {
                var bucket = buckets.Pop();
                var buffer = bucket.BucketBuffer;

                if (buffer == null)
                {
                    if (bucket.Entries != null)
                        yield return bucket;
                }
                else
                {
                    for (int i = buffer.Length - 1; i >= 0; i--) //keep the order of the buckets. Pop will get the items in the "correct" order
                        buckets.Push(buffer[i]);
                }
            } while (buckets.Count > 0);
        }

        private sealed class Bucket
        {
            public int Depth { get; }
            public int HashCodeSegmentSize { get; }
            public Bucket[] BucketBuffer { get; private set; }
            public MapLeafCollection Entries { get; private set; }
            public Map<TKey, TValue> Map { get; }
            public int Count { get; private set; }

            public Bucket(int depth, int hashCodeSegmentSize, Map<TKey, TValue> map)
            {
                if (map == null)
                    throw new ArgumentNullException(nameof(map));

                Depth = depth;
                HashCodeSegmentSize = hashCodeSegmentSize;
            }

            public bool IsCollapsed => BucketBuffer == null;

            public bool IsLeaf => Depth * HashCodeSegmentSize >= 32;

            public Bucket(int depth, int subBufferSize, Map<TKey, TValue> map, Entry item)
                : this(depth, subBufferSize, map)
            {
                Insert(item);
            }

            private int GetIndex(int hashCode) => unchecked((int)~0u >> (32 - HashCodeSegmentSize * Depth)) & hashCode;

            public bool Insert(Entry item, bool checkCollisions = true)
            {
                if (!item.HasValue)
                    throw new ArgumentException("Value expected.", nameof(item));

                if (BucketBuffer == null)
                {
                    int expansionThreshold = Map._expansionThreshold;

                    if (Entries == null)
                    {
                        if (IsLeaf || expansionThreshold > 1)
                        {
                            Entries = Map.CreateLeafCollection(Map._itemComparer);
                            Entries.Insert(item);
                        }
                        else
                            Extend();
                    }
                    else if (Count + 1 == expansionThreshold)
                    {
                        Extend();
                    }
                    else
                        Entries.Insert(item);

                    Count++;
                }

                if (BucketBuffer != null)
                {
                    int index = GetIndex(item.HashCode);

                    Bucket bucket = BucketBuffer[index];

                    if (bucket == null)
                    {
                        bucket = new Bucket(Depth + 1, HashCodeSegmentSize, Map);
                        BucketBuffer[index] = bucket;
                    }
                    if (bucket.Insert(item))
                    {
                        Count++;
                        return true;
                    }
                }

                return false;
            }

            public bool Remove(Entry key, bool all = false)
            {
                if (BucketBuffer == null)
                {
                    if (all)
                    {
                        bool removed = false;

                        while (Entries.Remove(key))
                        {
                            Count--;
                            removed = true;
                        }

                        return removed;
                    }
                    else if (Entries.Remove(key))
                    {
                        Count--;
                        return true;
                    }
                }
                else
                {
                    int index = GetIndex(key.HashCode);
                    var bucket = BucketBuffer[index];

                    if (bucket == null)
                        return false;

                    if (bucket.Remove(key, all))
                    {
                        if (--Count <= Map._collapseThreshold)
                            Collapse();

                        return true;
                    }
                }
                return false;
            }

            public bool Find(Entry key, out Entry result)
            {
                if (BucketBuffer == null)
                {
                    var entries = Entries;

                    if (entries == null)
                    {
                        result = default;
                        return false;
                    }

                    return entries.TryGet(key, out result);
                }
                else
                {
                    int index = GetIndex(key.HashCode);
                    var bucket = BucketBuffer[index];

                    if (bucket == null)
                    {
                        result = default;
                        return false;
                    }

                    return bucket.Find(key, out result);
                }
            }

            private void Extend()
            {
                BucketBuffer = new Bucket[1 << HashCodeSegmentSize];

                var e = Entries;

                Entries = null;

                foreach (var entry in e)
                    Insert(entry, false);
            }

            private void Collapse()
            {
                if (BucketBuffer == null) //nothing to collapse
                    return;

                if (Entries != null) //already collapsed
                    return;

                Entries = Map.CreateLeafCollection(Map._itemComparer);

                int bucketSize = BucketBuffer.Length;

                for (int i = 0; i < bucketSize; i++)
                {
                    var bucket = BucketBuffer[i];

                    if (bucket == null)
                        continue;

                    bucket.Collapse();

                    foreach (var bucketItem in bucket.Entries)
                        Entries.Insert(bucketItem, false);
                }

                BucketBuffer = null;
            }
        }

        protected struct Entry
        {
            public int HashCode { get; }
            public TKey Key { get; }
            public TValue Value { get; }
            public bool HasValue { get; }

            public Entry(int hashCode, TKey key)
            {
                HashCode = hashCode;
                Key = key;
                Value = default;
                HasValue = false;
            }

            public Entry(int hashCode, TKey key, TValue value)
                : this(hashCode, key)
            {
                Value = value;
                HashCode = hashCode;
                Key = key;
                HasValue = true;
            }

            public Entry(Entry oldEntry, TValue newValue)
                : this(oldEntry.HashCode, oldEntry.Key, newValue)
            { }

            public bool KeywiseEquals(Entry other, IEqualityComparer<TKey> keyComparer)
            {
                if (!(other.HashCode == HashCode && keyComparer.Equals(Key, other.Key))) //keys are not matching
                    return false;

                if (HasValue && other.HasValue) //if one of the two does not have a value, we do not check for that equality, otherwise, we do
                    if (!EqualityComparer<TValue>.Default.Equals(Value, other.Value))
                        return false;

                return true;
            }

            public override int GetHashCode()
            {
                return HashCode;
            }

            public override string ToString()
            {
                return HasValue ? Key + ", " + Value : Key?.ToString() ?? string.Empty;
            }

            public KeyValuePair<TKey, TValue> GetPair() => new KeyValuePair<TKey, TValue>(Key, Value);

            public static implicit operator KeyValuePair<TKey, TValue>(Entry item) => item.GetPair();
        }

        protected class ItemComparer : IEqualityComparer<Entry>
        {
            public IEqualityComparer<TKey> KeyComparer { get; }

            public ItemComparer(IEqualityComparer<TKey> keyComparer)
            {
                KeyComparer = keyComparer;
            }

            public bool Equals(Entry x, Entry y)
            {
                return x.KeywiseEquals(y, KeyComparer);
            }

            public int GetHashCode(Entry obj)
            {
                return obj.HashCode;
            }
        }

        protected abstract class MapLeafCollection : IEnumerable<Entry>
        {
            public abstract bool Insert(Entry item, bool checkCollisions = true);
            public abstract bool Remove(Entry item/*, bool removeAll*/);
            public abstract bool TryGet(Entry key, out Entry item);
            //public abstract IEnumerable<Entry> GetAll(Entry key);
            public abstract bool Contains(Entry key);
            //public abstract int Count(Entry key);

            public abstract IEnumerator<Entry> GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class ListMapLeafCollection : MapLeafCollection
        {
            private readonly List<Entry> _entries;
            public ItemComparer Comparer { get; }

            public ListMapLeafCollection(ItemComparer comparer)
            {
                Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
                _entries = new List<Entry>();
            }

            //public override int Count(Entry key)
            //{
            //    int count = 0;

            //    for (int i = _entries.Count - 1; i >= 0; i--)
            //        if (Comparer.Equals(_entries[i], key))
            //            count++;

            //    return count;
            //}

            public override bool Contains(Entry key)
            {
                for (int i = _entries.Count - 1; i >= 0; i--)
                    if (Comparer.Equals(_entries[i], key))
                        return true;

                return false;
            }

            public override IEnumerator<Entry> GetEnumerator()
            {
                return _entries.GetEnumerator();
            }

            public override bool Insert(Entry item, bool checkCollisions)
            {
                if (checkCollisions && Contains(item))
                    return false;

                _entries.Add(item);
                return true;
            }

            public override bool Remove(Entry item/*, bool removeAll*/)
            {
                bool success = false;

                for (int i = _entries.Count - 1; i >= 0; i--)
                    if (Comparer.Equals(_entries[i], item))
                    {
                        _entries.RemoveAt(i);

                        //if (!removeAll)
                        return true;

                        //success = true;
                    }

                return success;
            }

            public override bool TryGet(Entry key, out Entry item)
            {
                int c = _entries.Count;

                for (int i = 0; i < c; i++)
                {
                    Entry result = _entries[i];

                    if (Comparer.Equals(key, result))
                    {
                        item = result;
                        return true;
                    }
                }

                item = default;
                return false;
            }

            //public override IEnumerable<Entry> GetAll(Entry key)
            //{
            //    for (int i = _entries.Count - 1; i >= 0; i--)
            //    {
            //        var entry = _entries[i];

            //        if (Comparer.Equals(entry, key))
            //        {
            //            yield return entry;
            //        }
            //    }
            //}
        }
    }
}
