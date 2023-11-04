using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility.Collections.Adapters;

namespace Utility.Collections
{
    public struct ReadOnlyList<T> : IReadOnlyList<T>, IList<T>, IList
    {
        private readonly IList<T> _underlyingList;

        public ReadOnlyList(IList<T> underlyingList)
        {
            if (underlyingList == null)
                throw new ArgumentNullException(nameof(underlyingList));

            _underlyingList = underlyingList;
        }

        private ListAdapterNongeneric<T, IList<T>> ListHelper
        {
            get
            {
                if (_underlyingList == null)
                    throw new NullReferenceException();

                return new ListAdapterNongeneric<T, IList<T>>(_underlyingList);
            }
        }

        public T this[int index] => _underlyingList[index];

        public int Count => _underlyingList.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _underlyingList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _underlyingList.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return _underlyingList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _underlyingList.CopyTo(array, arrayIndex);
        }

        private Exception ReadOnlyException()
        {
            if (_underlyingList == null)
                throw new NullReferenceException();

            return new InvalidOperationException("The current list is read-only.");
        }

        #region IList<T>, ICollection<T>, IList, ICollection, IEnumerable
        T IList<T>.this[int index]
        {
            get => this[index];
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index does not fall into the range of the list.");

                throw ReadOnlyException();
            }
        }

        object IList.this[int index]
        {
            get => ListHelper[index];
            set
            {
                var lh = ListHelper;
                lh[index] = value;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {

                if (_underlyingList == null)
                    throw new NullReferenceException();

                return true;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                if (_underlyingList == null)
                    throw new NullReferenceException();

                return true;
            }
        }

        bool IList.IsFixedSize => _underlyingList.IsFixedSize();

        bool ICollection.IsSynchronized => ListHelper.IsSynchronized;

        object ICollection.SyncRoot => ListHelper.SyncRoot;

        void ICollection<T>.Add(T item)
        {
            throw ReadOnlyException();
        }

        int IList.Add(object value)
        {
            throw ReadOnlyException();
        }

        void ICollection<T>.Clear()
        {
            throw ReadOnlyException();
        }

        void IList.Clear()
        {
            throw ReadOnlyException();
        }

        bool IList.Contains(object value)
        {
            return ListHelper.Contains(value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ListHelper.CopyTo(array, index);
        }

        int IList.IndexOf(object value)
        {
            return ListHelper.IndexOf(value);
        }

        void IList<T>.Insert(int index, T item)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index does not fall into the range of the list.");

            throw ReadOnlyException();
        }

        void IList.Insert(int index, object value)
        {
            ListHelper.Insert(index, value);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw ReadOnlyException();
        }

        void IList.Remove(object value)
        {
            ListHelper.Remove(value);
        }

        void IList<T>.RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index does not fall into the range of the list.");

            throw ReadOnlyException();
        }

        void IList.RemoveAt(int index)
        {
            ListHelper.RemoveAt(index);
        }
        #endregion
    }
}
