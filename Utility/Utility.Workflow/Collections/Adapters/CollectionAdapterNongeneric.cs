using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility.Workflow.Collections.Adapters
{
    public struct CollectionAdapterNongeneric<TItem, TCollection> : ICollection
        where TCollection : ICollection<TItem>
    {
        private readonly TCollection _list;

        public CollectionAdapterNongeneric(TCollection list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _list = list;
        }

        public bool IsReadOnly => _list.IsReadOnly;

        public int Count => _list.Count;

        public bool IsSynchronized
        {
            get
            {
                if (_list == null)
                    throw new NullReferenceException();

                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                if (_list == null)
                    throw new NullReferenceException();

                return null;
            }
        }

        public int Add(object value)
        {
            if (_list == null)
                throw new NullReferenceException();

            _list.Add(Cast(value));

            return _list.Count - 1;
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(object value)
        {
            TItem v;

            if (_list == null)
                throw new NullReferenceException();

            if (!Cast(value, out v))
                return false;

            return _list.Contains(v);
        }

        public void CopyTo(Array array, int index)
        {

            if (_list == null)
                throw new NullReferenceException();

            if (array == null)
                throw new ArgumentNullException(nameof(array));

            TItem[] v = array as TItem[];

            if (v == null)
                throw new ArgumentException($"One-dimensional, zero-based array of type {typeof(TItem).FullName} expected.");

            _list.CopyTo(v, index);
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Remove(object value)
        {
            TItem v;

            if (_list == null)
                throw new NullReferenceException();

            if (!Cast(value, out v))
                return;

            _list.Remove(v);
        }

        private static TItem Cast(object value)
        {
            TItem it;

            if (!Cast(value, out it))
                throw new ArgumentException($"Value expected to be of type {typeof(TItem).FullName}.");

            return it;
        }

        private static bool Cast(object value, out TItem result)
        {
            if (value is TItem r)
            {
                result = r;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}
