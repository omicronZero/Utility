using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility.Collections.Adapters
{
    public struct ListAdapterNongeneric<TItem, TList> : IList
        where TList : IList<TItem>
    {
        private readonly TList _list;

        public ListAdapterNongeneric(TList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _list = list;
        }

        public object this[int index]
        {
            get => _list[index];
            set => _list[index] = Cast(value);
        }

        public bool IsFixedSize => false;

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
            if (_list == null)
                throw new NullReferenceException();

            TItem v;

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

        public int IndexOf(object value)
        {
            if (_list == null)
                throw new NullReferenceException();

            TItem v;

            if (!Cast(value, out v))
                return -1;

            return _list.IndexOf(v);
        }

        public void Insert(int index, object value)
        {
            if (_list == null)
                throw new NullReferenceException();

            _list.Insert(index, Cast(value));
        }

        public void Remove(object value)
        {
            if (_list == null)
                throw new NullReferenceException();

            TItem v;

            if (!Cast(value, out v))
                return;

            _list.Remove(v);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
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
