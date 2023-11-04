using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility.Collections.Adapters
{
    public abstract class ListBase<T> : CollectionBase<T>, IList<T>, IReadOnlyList<T>, IList
    {
        public abstract T this[int index] { get; set; }

        public abstract int IndexOf(T item);
        public abstract void Insert(int index, T item);
        public abstract void RemoveAt(int index);

        public override void Add(T item)
        {
            Insert(Count, item);
        }

        public override bool Remove(T item)
        {
            int index = IndexOf(item);

            if (index < 0)
                return false;

            RemoveAt(index);
            return true;
        }

        public override bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        protected virtual bool IsFixedSize => false;
        protected virtual bool IsSetterReadOnly => IsReadOnly;

        bool IList.IsFixedSize => IsFixedSize;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = CastItem(value);
        }

        int IList.Add(object value)
        {
            Add(CastItem(value));
            return Count;
        }

        bool IList.Contains(object value)
        {
            if (!(value is T v))
                return false;

            return Contains(v);
        }

        int IList.IndexOf(object value)
        {
            if (!(value is T v))
                return -1;

            return IndexOf(v);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, CastItem(value));
        }

        void IList.Remove(object value)
        {
            if (!(value is T v))
                return;

            Remove(v);
        }

        bool IList.IsReadOnly => IsReadOnly && IsSetterReadOnly;

        private static T CastItem(object value)
        {
            if (!(value is T v))
                throw new ArgumentException($"Item of type {typeof(T).FullName} expected.");

            return v;
        }
    }
}
