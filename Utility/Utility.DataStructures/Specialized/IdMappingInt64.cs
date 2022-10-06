using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility.Collections.Tools;

namespace Utility.DataStructures.Specialized
{
    public class IdMappingInt64<T>
    {
        private long _currentId;
        private readonly Dictionary<T, long> _map;
        private readonly Dictionary<long, T> _unmap;

        public IdMappingInt64(IEqualityComparer<T> equalityComparer)
        {
            if (equalityComparer == null)
                equalityComparer = EqualityComparer<T>.Default;

            _map = new Dictionary<T, long>(equalityComparer);
            _unmap = new Dictionary<long, T>();
        }

        private long AddCore(T value)
        {
            var v = _currentId++;

            _map.Add(value, v);
            _unmap.Add(v, value);

            return v;
        }

        public int Count => _map.Count;

        public long AddOrGet(T value)
        {
            if (_map.TryGetValue(value, out var v))
                return v;

            return AddCore(value);
        }

        public bool TryAdd(T value)
        {
            if (_map.ContainsKey(value))
                return false;

            AddCore(value);
            return true;
        }

        public bool TryAdd(T value, out long id)
        {
            if (_map.ContainsKey(value))
            {
                id = 0;
                return false;
            }

            id = AddCore(value);
            return true;
        }

        public bool TryGet(T value, out long id)
        {
            return _map.TryGetValue(value, out id);
        }

        public bool TryRemove(T value)
        {
            if (!_map.TryGetValue(value, out var id))
                return false;

            _unmap.Remove(id);
            _map.Remove(value);

            return true;

        }

        public bool TryRemove(T value, out long id)
        {
            if (!_map.TryGetValue(value, out id))
                return false;

            _unmap.Remove(id);
            _map.Remove(value);

            return true;

        }

        public bool TryRemoveById(long id)
        {
            if (!_unmap.TryGetValue(id, out var value))
                return false;

            _unmap.Remove(id);
            _map.Remove(value);

            return true;
        }

        public ICollection<T> GetValues()
        {
            return _map.Keys;
        }

        public ICollection<long> GetIds()
        {
            return _map.Values;
        }

        public bool TryRemoveById(long id, out T value)
        {
            if (!_unmap.TryGetValue(id, out value))
                return false;

            _unmap.Remove(id);
            _map.Remove(value);

            return true;
        }

        public bool TryGetById(long id, out T value)
        {
            return _unmap.TryGetValue(id, out value);
        }

        public void Clear(bool resetIdCounter)
        {
            _unmap.Clear();
            _map.Clear();

            if (resetIdCounter)
                _currentId = 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _map.Keys.CopyTo(array, arrayIndex);
        }

        public void CopyIdsTo(long[] array, int arrayIndex)
        {
            _map.Values.CopyTo(array, arrayIndex);
        }

        public bool Contains(T value) => _map.ContainsKey(value);

        public bool ContainsId(long id) => _unmap.ContainsKey(id);

        public bool Reassign(long id, T newValue, out T oldValue)
        {
            bool hadOldValue = _unmap.TryGetValue(id, out oldValue);

            if (_map.TryGetValue(newValue, out var newValueId) && newValueId != id)
                throw new ArgumentException("The new value to assign has already been registered with another id.");

            if (hadOldValue)
            {
                _unmap[id] = newValue;
                _map.Remove(oldValue);
            }

            _map[newValue] = id;

            return hadOldValue;
        }

        public ForwardDictionary AsForwardDict() => new ForwardDictionary(this);
        public BackwardDictionary AsBackwardDict() => new BackwardDictionary(this);

        public class ForwardDictionary : IDictionary<T, long>, IReadOnlyDictionary<T, long>
        {
            private readonly IdMappingInt64<T> _idMap;

            internal ForwardDictionary(IdMappingInt64<T> idMap)
            {
                _idMap = idMap ?? throw new ArgumentNullException(nameof(idMap));
            }

            public long this[T key]
            {
                get => _idMap._map[key];
                set => throw CollectionExceptions.ReadOnlyException();
            }

            public ICollection<T> Keys => _idMap._map.Keys;

            public ICollection<long> Values => _idMap._map.Values;

            public int Count => _idMap.Count;

            bool ICollection<KeyValuePair<T, long>>.IsReadOnly => true;

            IEnumerable<T> IReadOnlyDictionary<T, long>.Keys => _idMap._map.Keys;
            IEnumerable<long> IReadOnlyDictionary<T, long>.Values => _idMap._map.Values;

            public bool TryGetValue(T key, out long value)
            {
                return _idMap._map.TryGetValue(key, out value);
            }

            public bool Contains(KeyValuePair<T, long> item)
            {
                return _idMap._map.TryGetValue(item.Key, out var id) && id == item.Value;
            }

            public bool ContainsKey(T key)
            {
                return _idMap._map.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<T, long>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<T, long>>)_idMap._map).CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<T, long>> GetEnumerator()
            {
                return _idMap._map.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            bool IDictionary<T, long>.Remove(T key)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            bool ICollection<KeyValuePair<T, long>>.Remove(KeyValuePair<T, long> item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            void IDictionary<T, long>.Add(T key, long value)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            void ICollection<KeyValuePair<T, long>>.Add(KeyValuePair<T, long> item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            void ICollection<KeyValuePair<T, long>>.Clear()
            {
                throw CollectionExceptions.ReadOnlyException();
            }
        }

        public class BackwardDictionary : IDictionary<long, T>, IReadOnlyDictionary<long, T>
        {
            private readonly IdMappingInt64<T> _idMap;

            internal BackwardDictionary(IdMappingInt64<T> idMap)
            {
                _idMap = idMap ?? throw new ArgumentNullException(nameof(idMap));
            }

            public T this[long key]
            {
                get => _idMap._unmap[key];
                set => throw CollectionExceptions.ReadOnlyException();
            }

            public ICollection<long> Keys => _idMap._unmap.Keys;

            public ICollection<T> Values => _idMap._unmap.Values;

            public int Count => _idMap._unmap.Count;

            bool ICollection<KeyValuePair<long, T>>.IsReadOnly => true;

            IEnumerable<long> IReadOnlyDictionary<long, T>.Keys => _idMap._unmap.Keys;
            IEnumerable<T> IReadOnlyDictionary<long, T>.Values => _idMap._unmap.Values;

            public bool TryGetValue(long key, out T value)
            {
                return _idMap._unmap.TryGetValue(key, out value);
            }

            public bool Contains(KeyValuePair<long, T> item)
            {
                return _idMap._unmap.TryGetValue(item.Key, out var id) && _idMap._map.Comparer.Equals(id, item.Value);
            }

            public bool ContainsKey(long key)
            {
                return _idMap._unmap.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<long, T>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<long, T>>)_idMap._unmap).CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<long, T>> GetEnumerator()
            {
                return _idMap._unmap.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            bool IDictionary<long, T>.Remove(long key)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            bool ICollection<KeyValuePair<long, T>>.Remove(KeyValuePair<long, T> item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            void IDictionary<long, T>.Add(long key, T value)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            void ICollection<KeyValuePair<long, T>>.Add(KeyValuePair<long, T> item)
            {
                throw CollectionExceptions.ReadOnlyException();
            }

            void ICollection<KeyValuePair<long, T>>.Clear()
            {
                throw CollectionExceptions.ReadOnlyException();
            }
        }
    }
}
