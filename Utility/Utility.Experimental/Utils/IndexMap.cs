using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class IndexMap<T>
    {
        private readonly Dictionary<ItemKey, ItemKey> _keyMap;
        private long _index;

        public int Count => _keyMap.Count / 2;

        public IndexMap()
        {
            _keyMap = new Dictionary<ItemKey, ItemKey>();
        }

        public bool TryGet(long index, out T item)
        {
            ItemKey k;

            if (!_keyMap.TryGetValue(index, out k))
            {
                item = default;
                return false;
            }

            item = k.Item;
            return true;
        }

        public bool TryGet(T item, out long index)
        {
            ItemKey k;

            if (!_keyMap.TryGetValue(item, out k))
            {
                index = -1;
                return false;
            }

            index = k.Index.Value;
            return true;
        }

        public bool Add(T item, out long index)
        {
            if (TryGet(item, out index))
                return false;

            index = _index++;

            _keyMap.Add(index, item);
            _keyMap.Add(item, index);

            return true;
        }

        public bool Remove(T item)
        {
            long index;

            if (!TryGet(item, out index))
                return false;

            _keyMap.Remove(item);
            _keyMap.Remove(index);

            return true;
        }

        public bool Remove(long index)
        {
            T item;

            if (!TryGet(index, out item))
                return false;

            _keyMap.Remove(item);
            _keyMap.Remove(index);

            return true;
        }

        public long this[T item]
        {
            get
            {
                long index;

                if (!TryGet(item, out index))
                    throw new ArgumentException("The specified item is not a member of the current instance.", nameof(item));

                return index;
            }
        }

        public T this[long index]
        {
            get
            {
                T item;

                if (!TryGet(index, out item))
                    throw new ArgumentException("The specified index is not a member of the current instance.", nameof(index));

                return item;
            }
        }

        private struct ItemKey : IEquatable<ItemKey>
        {
            private static readonly ReferenceComparer<T> ItemComparer = ReferenceComparer<T>.Default;

            public T Item { get; }
            public long? Index { get; }

            public ItemKey(T item)
            {
                Item = item;
                Index = null;
            }

            public ItemKey(long index)
            {
                Item = default;
                Index = index;
            }

            public override int GetHashCode()
            {
                return Index?.GetHashCode() ?? ItemComparer.GetHashCode(Item);
            }

            public override bool Equals(object obj)
            {
                return obj != null && obj is ItemKey k && Equals(k);
            }

            public bool Equals(ItemKey other)
            {
                if (Index != null)
                    return other.Index != null && other.Index.Value == Index.Value;
                else
                    return other.Index == null && ItemComparer.Equals(Item, other.Item);
            }

            public static implicit operator ItemKey(long index) => new ItemKey(index);
            public static implicit operator ItemKey(T item) => new ItemKey(item);
        }
    }
}
