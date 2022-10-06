using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Reflection;

namespace Utility.Collections
{
    public static class ListExtensions
    {
        public static bool IsSetterReadonly<T>(this IList<T> instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (!(instance is IList c))
                return instance.IsReadOnly;

            return c.IsReadOnly;
        }

        public static bool IsFixedSize<T>(this IList<T> instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return (instance is IList c) && c.IsFixedSize;
        }

        public static SingleItemList<T> Single<T>(T item)
        {
            return new SingleItemList<T>(item);
        }

        public static IList<TResult> CastList<TSource, TResult>(this IList<TSource> list)
            where TResult : TSource
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return list as IList<TResult> ?? new CastListWrapper<TSource, TResult>(list);
        }

        public static IList<TResult> CastList<TResult>(this IList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            //The creation of the cast list via CastList<TResult> is more inefficient than directly calling CastList<TSource, TResult> but the case that IList is an IList<T> is caught by this call
            Type[] sourceTypes = TypeReflectionExtensions.GetGenericImplementations(list.GetType(), typeof(IList<>)).Select((s) => s.GetGenericArguments()[0]).ToArray();

            if (sourceTypes.Length == 1)
            {
                if (sourceTypes[0].IsAssignableFrom(typeof(TResult)))
                    return (IList<TResult>)Activator.CreateInstance(typeof(CastListWrapper<,>).MakeGenericType(sourceTypes[0], typeof(TResult)), list);
            }

            return list as IList<TResult> ?? new CastListWrapper<TResult>(list);
        }

        public class SingleItemList<T> : IList<T>, IList
        {
            private readonly T _item;

            internal SingleItemList(T item)
            {
                _item = item;
            }

            public T this[int index]
            {
                get
                {
                    Util.ValidateNamedIndex(index, 1, lengthName: nameof(Count));
                    return _item;
                }
                set => throw ReadOnly();
            }

            public int Count => 1;

            public bool IsReadOnly => true;

            public void Add(T item)
            {
                throw ReadOnly();
            }

            public void Clear()
            {
                throw ReadOnly();
            }

            public bool Contains(T item)
            {
                return EqualityComparer<T>.Default.Equals(_item, item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                Util.ValidateRange(array, arrayIndex, 1);
                array[arrayIndex] = _item;
            }

            public IEnumerator<T> GetEnumerator()
            {
                yield return _item;
            }

            public int IndexOf(T item)
            {
                return Contains(item) ? 0 : -1;
            }

            public void Insert(int index, T item)
            {
                throw ReadOnly();
            }

            public bool Remove(T item)
            {
                throw ReadOnly();
            }

            public void RemoveAt(int index)
            {
                throw ReadOnly();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private Exception ReadOnly()
            {
                return new InvalidOperationException("The current list is read-only.");
            }

            #region IList, ICollection

            bool IList.IsFixedSize => true;

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

        private sealed class CastListWrapper<TResult> : ListBase<TResult>
        {
            private readonly IList _innerList;

            public CastListWrapper(IList innerList)
            {
                if (innerList == null)
                    throw new ArgumentNullException(nameof(innerList));

                _innerList = innerList;
            }

            public override TResult this[int index]
            {
                get => (TResult)_innerList[index];
                set => _innerList[index] = value;
            }

            public override int Count => _innerList.Count;

            public override bool IsReadOnly => _innerList.IsReadOnly || _innerList.IsFixedSize;

            public override void Clear()
            {
                _innerList.Clear();
            }

            public override IEnumerator<TResult> GetEnumerator()
            {
                return _innerList.Cast<TResult>().GetEnumerator();
            }

            public override int IndexOf(TResult item)
            {
                return _innerList.IndexOf(item);
            }

            public override void Insert(int index, TResult item)
            {
                _innerList.Insert(index, item);
            }

            public override void RemoveAt(int index)
            {
                _innerList.RemoveAt(index);
            }

            public override bool Remove(TResult item)
            {
                int c = _innerList.Count;
                _innerList.Remove(item);

                return c != _innerList.Count;
            }

            protected override bool IsFixedSize => _innerList.IsFixedSize;
            protected override object SyncRoot => _innerList.IsSynchronized ? _innerList.SyncRoot : null;

            public override bool Contains(TResult item)
            {
                return _innerList.Contains(item);
            }

            public override void Add(TResult item)
            {
                _innerList.Add(item);
            }

            public override void CopyTo(TResult[] array, int arrayIndex)
            {
                _innerList.CopyTo(array, arrayIndex);
            }
        }

        private sealed class CastListWrapper<TSource, TResult> : ListBase<TResult>
            where TResult : TSource
        {
            private readonly IList<TSource> _innerList;

            public CastListWrapper(IList<TSource> innerList)
            {
                if (innerList == null)
                    throw new ArgumentNullException(nameof(innerList));

                _innerList = innerList;
            }

            public override TResult this[int index]
            {
                get => (TResult)_innerList[index];
                set => _innerList[index] = value;
            }

            public override int Count => _innerList.Count;

            public override bool IsReadOnly => _innerList.IsReadOnly;

            public override void Clear()
            {
                _innerList.Clear();
            }

            public override IEnumerator<TResult> GetEnumerator()
            {
                return _innerList.Cast<TResult>().GetEnumerator();
            }

            public override int IndexOf(TResult item)
            {
                return _innerList.IndexOf(item);
            }

            public override void Insert(int index, TResult item)
            {
                _innerList.Insert(index, item);
            }

            public override void RemoveAt(int index)
            {
                _innerList.RemoveAt(index);
            }

            public override bool Remove(TResult item)
            {
                int c = _innerList.Count;
                _innerList.Remove(item);

                return c != _innerList.Count;
            }

            protected override bool IsFixedSize => _innerList.IsFixedSize();
            protected override object SyncRoot => _innerList is IList l && l.IsSynchronized ? l.SyncRoot : null;

            public override bool Contains(TResult item)
            {
                return _innerList.Contains(item);
            }

            public override void Add(TResult item)
            {
                _innerList.Add(item);
            }

            public override void CopyTo(TResult[] array, int arrayIndex)
            {
                TSource[] src = array as TSource[];

                if (src != null)
                    _innerList.CopyTo(src, arrayIndex);
                else
                {
                    base.CopyTo(array, arrayIndex);
                }
            }
        }
    }
}
