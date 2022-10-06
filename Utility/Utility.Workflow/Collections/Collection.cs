using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility.Collections
{
    //TODO: inherit from ListBase
    [Serializable]
    public class Collection<T> : IList<T>, IReadOnlyList<T>, IList
    {
        private IList<T> _internalList;

        protected IList<T> InternalList
        {
            get { return _internalList; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                IList<T> l = _internalList;

                _internalList = value;

                try
                {
                    OnInternalListChanged();
                }
                catch
                {
                    _internalList = l;
                    throw;
                }
            }
        }

        public Collection()
            : this(new List<T>())
        { }

        public Collection(IList<T> internalList)
        {
            if (internalList == null)
                throw new ArgumentNullException(nameof(internalList));

            InternalList = internalList;
        }

        protected virtual void OnInternalListChanged()
        { }

        protected virtual int IndexOfItem(T item)
        {
            return _internalList.IndexOf(item);
        }

        protected virtual bool ContainsItem(T item)
        {
            return IndexOfItem(item) >= 0;
        }

        protected virtual void InsertItem(int index, T item)
        {
            _internalList.Insert(index, item);
        }

        protected virtual void SetItem(int index, T item)
        {
            _internalList[index] = item;
        }

        protected virtual void RemoveItem(int index)
        {
            _internalList.RemoveAt(index);
        }

        protected virtual void ClearItems()
        {
            _internalList.Clear();
        }

        public T this[int index]
        {
            get => _internalList[index];
            set => SetItem(index, value);
        }

        public int Count => _internalList.Count;

        bool ICollection<T>.IsReadOnly => _internalList.IsReadOnly;

        public void Add(T item)
        {
            InsertItem(Count, item);
        }

        public void Clear()
        {
            ClearItems();
        }

        public bool Contains(T item)
        {
            return ContainsItem(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return IndexOfItem(item);
        }

        public void Insert(int index, T item)
        {
            InsertItem(index, item);
        }

        public bool Remove(T item)
        {
            int ind = IndexOfItem(item);

            if (ind < 0)
                return false;

            RemoveItem(ind);

            return true;
        }

        public void RemoveAt(int index)
        {
            RemoveItem(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        #region IList, ICollection

        bool IList.IsReadOnly => _internalList.IsReadOnly;

        bool IList.IsFixedSize => _internalList.IsFixedSize();

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = Convert(value);
        }

        int IList.Add(object value)
        {
            Add(Convert(value));
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            return value is T v && Contains(v);
        }

        int IList.IndexOf(object value)
        {
            return value is T v ? IndexOf(v) : -1;
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, Convert(value));
        }

        void IList.Remove(object value)
        {
            Remove(Convert(value));
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (!(array is T[] arr))
                throw new ArgumentException($"Expected a one-dimensional array with element type { typeof(T).FullName }.");
            CopyTo(arr, index);
        }

        private static T Convert(object value)
        {
            if (!(value is T v))
                throw new ArgumentException($"Expected a value of type { typeof(T).FullName }.");

            return v;
        }

        #endregion
    }
}
