using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility.Collections.Tools;

namespace Utility.Collections
{
    public static class SelectorList
    {
        public static SelectorList<T, TUnderlying> SelectList<T, TUnderlying>(this IList<TUnderlying> underlyingList, Func<TUnderlying, T> selector)
        {
            return SelectList(underlyingList, selector, null);
        }

        public static SelectorList<T, TUnderlying> SelectList<T, TUnderlying>(this IList<TUnderlying> underlyingList, Func<TUnderlying, T> selector, Func<T, TUnderlying> converter)
        {
            return new SelectorList<T, TUnderlying>(underlyingList, selector, converter);
        }
    }

    public struct SelectorList<T, TUnderlying> : IList<T>, IList
    {
        private readonly IList<TUnderlying> _underlyingList;
        private readonly Func<TUnderlying, T> _selector;
        private readonly Func<T, TUnderlying> _converter;

        public SelectorList(IList<TUnderlying> underlyingList, Func<TUnderlying, T> selector)
            : this(underlyingList, selector, null)
        { }

        public SelectorList(IList<TUnderlying> underlyingList, Func<TUnderlying, T> selector, Func<T, TUnderlying> converter)
        {
            if (underlyingList == null)
                throw new ArgumentNullException(nameof(underlyingList));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _underlyingList = underlyingList;
            _selector = selector;

            if (converter == null)
            {
                _converter = null;
            }
            else
            {
                _converter = (v) =>
                    {
                        try
                        {
                            return converter(v);
                        }
                        catch (FormatException ex)
                        {
                            throw new ArgumentException("Bad format.", ex);
                        }
                    };
            }
        }

        private ListHelper<T, SelectorList<T, TUnderlying>> ListHelper
        {
            get
            {
                if (_underlyingList == null)
                    throw new NullReferenceException();

                return new ListHelper<T, SelectorList<T, TUnderlying>>(this);
            }
        }

        private void CheckReadonly()
        {
            if (IsReadOnly)
                throw CollectionExceptions.ReadOnlyException();
        }

        public T this[int index]
        {
            get
            {
                if (_underlyingList == null)
                    throw new NullReferenceException();

                return _selector(_underlyingList[index]);
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "The specified index does not fall into the range of the list.");

                CheckReadonly();

                _underlyingList[index] = _converter(value);
            }
        }

        public int Count
        {
            get
            {
                if (_underlyingList == null)
                    throw new NullReferenceException();

                return _underlyingList.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                if (_underlyingList == null)
                    throw new NullReferenceException();

                return _converter == null || _underlyingList.IsReadOnly;
            }
        }

        public void Add(T item)
        {
            CheckReadonly();

            _underlyingList.Add(_converter(item));
        }

        public void Clear()
        {
            CheckReadonly();

            _underlyingList.Clear();
        }

        public bool Contains(T item)
        {
            if (_underlyingList == null)
                throw new NullReferenceException();

            if (_converter != null)
            {
                TUnderlying converted;

                try
                {
                    converted = _converter(item);
                }
                catch (ArgumentException)
                {
                    return false;
                }

                return _underlyingList.Contains(converted);
            }
            else
            {
                var comparer = EqualityComparer<T>.Default;

                foreach (TUnderlying v in _underlyingList)
                {
                    if (comparer.Equals(_selector(v), item))
                        return true;
                }
                return false;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_underlyingList == null)
                throw new NullReferenceException();

            SelectorCollection.SelectCollection(_underlyingList, _selector).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_underlyingList == null)
                throw new NullReferenceException();

            foreach (TUnderlying item in _underlyingList)
                yield return _selector(item);
        }

        public int IndexOf(T item)
        {
            if (_underlyingList == null)
                throw new NullReferenceException();

            if (_converter != null)
            {
                TUnderlying converted;

                try
                {
                    converted = _converter(item);
                }
                catch (ArgumentException)
                {
                    return -1;
                }

                return _underlyingList.IndexOf(converted);
            }
            else
            {
                var comparer = EqualityComparer<T>.Default;
                int ind = 0;
                foreach (TUnderlying v in _underlyingList)
                {
                    if (comparer.Equals(_selector(v), item))
                        return ind;
                    ind++;
                }
                return -1;
            }
        }

        public void Insert(int index, T item)
        {
            if (_underlyingList == null)
                throw new NullReferenceException();

            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index does not fall into the range of the list.");

            CheckReadonly();

            _underlyingList.Insert(index, _converter(item));
        }

        public bool Remove(T item)
        {
            if (_underlyingList == null)
                throw new NullReferenceException();

            CheckReadonly();

            TUnderlying underlying;

            try
            {
                underlying = _converter(item);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return _underlyingList.Remove(underlying);
        }

        public void RemoveAt(int index)
        {
            if (_underlyingList == null)
                throw new NullReferenceException();

            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index does not fall into the range of the list.");

            CheckReadonly();

            _underlyingList.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsFixedSize => ListHelper.IsFixedSize;

        public bool IsSynchronized => ListHelper.IsSynchronized;

        public object SyncRoot => ListHelper.SyncRoot;

        object IList.this[int index]
        {
            get => ListHelper[index];
            set
            {
                var lh = ListHelper;
                lh[index] = value;
            }
        }

        int IList.Add(object value)
        {
            return ListHelper.Add(value);
        }

        bool IList.Contains(object value)
        {
            return ListHelper.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ListHelper.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            ListHelper.Insert(index, value);
        }

        void IList.Remove(object value)
        {
            ListHelper.Remove(value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ListHelper.CopyTo(array, index);
        }
    }
}
