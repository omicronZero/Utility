using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility.Workflow.Collections.Adapters;

namespace Utility.Collections
{
    public struct ReadOnlyCollection<T> : IReadOnlyCollection<T>, ICollection<T>, ICollection
    {
        private readonly ICollection<T> _underlyingList;

        public ReadOnlyCollection(IList<T> underlyingList)
        {
            if (underlyingList == null)
                throw new ArgumentNullException(nameof(underlyingList));

            _underlyingList = underlyingList;
        }

        private CollectionAdapterNongeneric<T, ICollection<T>> ListHelper
        {
            get
            {
                if (_underlyingList == null)
                    throw new NullReferenceException();

                return new CollectionAdapterNongeneric<T, ICollection<T>>(_underlyingList);
            }
        }

        public int Count => _underlyingList.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _underlyingList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                if (_underlyingList == null)
                    throw new NullReferenceException();

                return true;
            }
        }

        bool ICollection.IsSynchronized => ListHelper.IsSynchronized;

        object ICollection.SyncRoot => ListHelper.SyncRoot;

        void ICollection<T>.Add(T item)
        {
            throw ReadOnlyException();
        }

        void ICollection<T>.Clear()
        {
            throw ReadOnlyException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ListHelper.CopyTo(array, index);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw ReadOnlyException();
        }
        #endregion
    }
}
