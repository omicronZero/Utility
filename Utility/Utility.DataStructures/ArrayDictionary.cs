using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections;
using Utility.Collections.Tools;
using Utility.Workflow.Collections.Adapters;

namespace Utility.DataStructures
{
    /// <summary>
    /// A dictionary based on a sorting on the keys' hash codes which are supplied by the dictionary's key comparer.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used to access a stored value.</typeparam>
    /// <typeparam name="TValue">The type of the value to be stored in the dictionary.</typeparam>
    public class ArrayDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly List<Entry> _items;
        public IEqualityComparer<TKey> KeyComparer { get; }

        public ArrayDictionary(IEqualityComparer<TKey> keyComparer = null)
        {
            _items = new List<Entry>();
            KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        }

        public ArrayDictionary(int capacity, IEqualityComparer<TKey> keyComparer = null)
        {
            _items = new List<Entry>(capacity);
            KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        }

        public ArrayDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> keyComparer = null)
        {
            KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _items = new List<Entry>(collection.Select((s) => new Entry(keyComparer.GetHashCode(), s.Key, s.Value)));
        }

        private bool FindItem(TKey key, out Entry item)
        {
            return FindItem(key, out item, out _);
        }

        private bool FindItem(TKey key, out Entry item, out int itemIndex)
        {
            return FindItem(KeyComparer.GetHashCode(key), key, out item, out itemIndex);
        }

        private bool FindItem(int hashCode, TKey key, out Entry item)
        {
            return FindItem(hashCode, key, out item, out _);
        }

        private bool FindItem(int hashCode, TKey key, out Entry item, out int itemIndex)
        {
            int index = _items.BinarySearch(new Entry(hashCode, key, default));

            itemIndex = index;

            if (index >= 0)
            {
                int c = _items.Count;

                for (int i = index; i < c; i++)
                {
                    var other = _items[i];

                    if (other.HashCode != hashCode)
                        break;

                    if (KeyComparer.Equals(other.Key, key))
                    {
                        item = other;
                        return true;
                    }
                }
            }

            item = default;
            return false;
        }

        private bool SetItem(TKey key, TValue value, bool overwrite = false)
        {
            return InsertItem(new Entry(KeyComparer.GetHashCode(key), key, value), overwrite);
        }

        private bool InsertItem(Entry item, bool overwrite = false)
        {
            int index = _items.BinarySearch(item);

            if (index >= 0)
            {
                int c = _items.Count;

                for (; index < c; index++)
                {
                    var other = _items[index];

                    if (other.HashCode != item.HashCode)
                        break;

                    if (KeyComparer.Equals(other.Key, item.Key))
                    {
                        if (overwrite)
                            _items[index] = item;

                        return overwrite;
                    }
                }
            }
            else
                index = ~index;

            _items.Insert(index, item);
            return true;
        }

        public TValue this[TKey key]
        {
            get
            {
                Entry item;

                if (!FindItem(key, out item))
                    throw new KeyNotFoundException();

                return item.Value;
            }
            set
            {
                SetItem(key, value, true);
            }
        }

        public IList<TKey> Keys => new KeyCollection(this);

        public IList<TValue> Values => _items.SelectList((e) => e.Value);

        public int Count => _items.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (!SetItem(key, value))
                throw new ArgumentException("An element with the same key already exists in the dictionary.", nameof(key));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _items.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            Entry entry;

            return FindItem(item.Key, out entry) && EqualityComparer<TValue>.Default.Equals(entry.Value, item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return FindItem(key, out _);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Util.ValidateNamedRange(array, arrayIndex, _items.Count, indexName: nameof(arrayIndex));

            foreach (var entry in this)
                array[arrayIndex++] = entry;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var item in _items)
                yield return item.GetPair();
        }

        public bool Remove(TKey key)
        {
            int index;

            if (!FindItem(key, out _, out index))
                return false;

            _items.RemoveAt(index);
            return true;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            int index;
            Entry entry;

            if (!FindItem(item.Key, out entry, out index) || EqualityComparer<TValue>.Default.Equals(entry.Value, item.Value))
                return false;

            _items.RemoveAt(index);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Entry entry;

            if (!FindItem(key, out entry))
            {
                value = default;
                return false;
            }

            value = entry.Value;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        private struct Entry : IComparable<Entry>
        {
            public int HashCode { get; }
            public TKey Key { get; }
            public TValue Value { get; }

            public Entry(int hashCode, TKey key, TValue value)
            {
                HashCode = hashCode;
                Key = key;
                Value = value;
            }

            public KeyValuePair<TKey, TValue> GetPair() => new KeyValuePair<TKey, TValue>(Key, Value);

            public int CompareTo(Entry other)
            {
                return HashCode.CompareTo(other.HashCode);
            }
        }

        private sealed class KeyCollection : ListBase<TKey>
        {
            public ArrayDictionary<TKey, TValue> Dictionary { get; }

            public KeyCollection(ArrayDictionary<TKey, TValue> dictionary)
            {
                Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            public override int Count => Dictionary.Count;

            public override bool IsReadOnly => true;

            public override TKey this[int index]
            {
                get
                {
                    return Dictionary._items[index].Key;
                }
                set => throw CollectionExceptions.ReadOnlyException();
            }

            public override int IndexOf(TKey item)
            {
                int index;

                if (!Dictionary.FindItem(item, out _, out index))
                    return -1;

                return index;
            }

            public override void Insert(int index, TKey item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public override void RemoveAt(int index)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public override void Add(TKey item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public override void Clear()
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public override bool Contains(TKey item)
            {
                return IndexOf(item) >= 0;
            }

            public override void CopyTo(TKey[] array, int arrayIndex)
            {
                CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);
            }

            public override bool Remove(TKey item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            public override IEnumerator<TKey> GetEnumerator()
            {
                return Dictionary._items.Select((s) => s.Key).GetEnumerator();
            }
        }
    }
}
