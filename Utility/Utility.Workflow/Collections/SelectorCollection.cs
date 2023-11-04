using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility.Collections.Adapters;
using Utility.Collections.Tools;

namespace Utility.Collections
{
    public static class SelectorCollection
    {
        public static SelectorCollection<T, TUnderlyingCollection> SelectCollection<T, TUnderlyingCollection>(this ICollection<TUnderlyingCollection> underlyingCollection, Func<TUnderlyingCollection, T> selector)
        {
            return new SelectorCollection<T, TUnderlyingCollection>(underlyingCollection, selector);
        }

        public static SelectorCollection<T, TUnderlyingCollection> SelectCollection<T, TUnderlyingCollection>(this ICollection<TUnderlyingCollection> underlyingCollection, Func<TUnderlyingCollection, T> selector, Func<T, TUnderlyingCollection> converter)
        {
            return new SelectorCollection<T, TUnderlyingCollection>(underlyingCollection, selector, converter);
        }
    }

    public struct SelectorCollection<T, TUnderlying> : ICollection<T>, ICollection, IReadOnlyCollection<T>
    {
        private readonly ICollection<TUnderlying> _underlyingCollection;
        private readonly Func<TUnderlying, T> _selector;
        private readonly Func<T, TUnderlying> _converter;

        public SelectorCollection(ICollection<TUnderlying> underlyingCollection, Func<TUnderlying, T> selector)
            : this(underlyingCollection, selector, null)
        { }

        public SelectorCollection(ICollection<TUnderlying> underlyingCollection, Func<TUnderlying, T> selector, Func<T, TUnderlying> converter)
        {
            if (underlyingCollection == null)
                throw new ArgumentNullException(nameof(underlyingCollection));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _underlyingCollection = underlyingCollection;
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

        private CollectionAdapterNongeneric<T, SelectorCollection<T, TUnderlying>> CollectionHelper
        {
            get
            {
                if (_underlyingCollection == null)
                    throw new NullReferenceException();

                return new CollectionAdapterNongeneric<T, SelectorCollection<T, TUnderlying>>(this);
            }
        }

        public int Count
        {
            get
            {
                if (_underlyingCollection == null)
                    throw new NullReferenceException();

                return _underlyingCollection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                if (_underlyingCollection == null)
                    throw new NullReferenceException();

                return _converter == null || _underlyingCollection.IsReadOnly;
            }
        }

        private void CheckReadonly()
        {
            if (_underlyingCollection == null)
                throw new NullReferenceException();

            if (IsReadOnly)
                throw CollectionExceptions.ReadOnlyException();
        }

        //for Clear, Remove, Add: read-only check on the underlying collection is omitted as it may change in the meantime and will be caught anyways by the underlying collection
        public void Add(T item)
        {
            CheckReadonly();

            _underlyingCollection.Add(_converter(item));
        }

        public void Clear()
        {
            CheckReadonly();

            _underlyingCollection.Clear();
        }

        public bool Remove(T item)
        {
            CheckReadonly();

            TUnderlying v;
            try
            {
                v = _converter(item);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return _underlyingCollection.Remove(v);
        }

        public bool Contains(T item)
        {
            if (_underlyingCollection == null)
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

                return _underlyingCollection.Contains(converted);
            }

            Func<TUnderlying, T> selector = _selector;

            return _underlyingCollection.Any((it) => EqualityComparer<T>.Default.Equals(selector(it), item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_underlyingCollection == null)
                throw new NullReferenceException();

            Util.ValidateNamedRange(array, arrayIndex, _underlyingCollection.Count, indexName: nameof(arrayIndex));

            foreach (TUnderlying v in _underlyingCollection)
                array[arrayIndex++] = _selector(v);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_underlyingCollection == null)
                throw new NullReferenceException();

            return _underlyingCollection.Select(_selector).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        int ICollection.Count => CollectionHelper.Count;

        bool ICollection.IsSynchronized => CollectionHelper.IsSynchronized;

        object ICollection.SyncRoot => CollectionHelper.SyncRoot;

        void ICollection.CopyTo(Array array, int index)
        {
            CollectionHelper.CopyTo(array, index);
        }
    }
}
