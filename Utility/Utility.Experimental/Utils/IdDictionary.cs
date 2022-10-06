using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class IdDictionary<TEntry, TId> : IDictionary<TEntry, TId>, IReadOnlyDictionary<TEntry, TId>
    {
        private readonly Dictionary<TEntry, TId> _underlyingDictionary;
        private Func<TId> _nextId;

        public IdDictionary(Func<TId> nextId)
            : this(nextId, null)
        { }

        public IdDictionary(Func<TId> nextId, IEqualityComparer<TEntry> comparer)
        {
            if (nextId == null)
                throw new ArgumentNullException(nameof(nextId));

            _underlyingDictionary = new Dictionary<TEntry, TId>(comparer);
            _nextId = nextId;
        }

        TId IDictionary<TEntry, TId>.this[TEntry key]
        {
            get => this[key];
            set => throw ReadonlyException();
        }

        public TId this[TEntry key]
        {
            get => _underlyingDictionary[key];
        }

        public TId Register(TEntry instance)
        {
            TId id = _nextId();

            _underlyingDictionary.Add(instance, id);

            return id;
        }

        public void Unregister(TEntry instance)
        {
            _underlyingDictionary.Remove(instance);
        }

        public ICollection<TEntry> Keys => _underlyingDictionary.Keys;

        public ICollection<TId> Values => _underlyingDictionary.Values;

        public int Count => _underlyingDictionary.Count;

        public bool IsReadOnly => true;

        IEnumerable<TEntry> IReadOnlyDictionary<TEntry, TId>.Keys => Keys;

        IEnumerable<TId> IReadOnlyDictionary<TEntry, TId>.Values => Values;

        void IDictionary<TEntry, TId>.Add(TEntry key, TId value)
        {
            throw ReadonlyException();
        }

        public bool Contains(KeyValuePair<TEntry, TId> item)
        {
            return ((IDictionary<TEntry, TId>)_underlyingDictionary).Contains(item);
        }

        public bool ContainsKey(TEntry key)
        {
            return _underlyingDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TEntry, TId>[] array, int arrayIndex)
        {
            ((IDictionary<TEntry, TId>)_underlyingDictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TEntry, TId>> GetEnumerator()
        {
            return _underlyingDictionary.GetEnumerator();
        }

        bool IDictionary<TEntry, TId>.Remove(TEntry key)
        {
            throw ReadonlyException();
        }

        bool ICollection<KeyValuePair<TEntry, TId>>.Remove(KeyValuePair<TEntry, TId> item)
        {
            throw ReadonlyException();
        }

        void ICollection<KeyValuePair<TEntry, TId>>.Add(KeyValuePair<TEntry, TId> item)
        {
            throw ReadonlyException();
        }

        void ICollection<KeyValuePair<TEntry, TId>>.Clear()
        {
            throw ReadonlyException();
        }

        public bool TryGetValue(TEntry key, out TId value)
        {
            return _underlyingDictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_underlyingDictionary).GetEnumerator();
        }

        private static Exception ReadonlyException()
        {
            return new NotSupportedException("The current dictionary is read-only.");
        }

        public static IdDictionary<TEntry, TId> CreateReferenceDictionary(Func<TId> nextId)
        {
            return new IdDictionary<TEntry, TId>(nextId, ReferenceComparer<TEntry>.Default);
        }
    }
}
