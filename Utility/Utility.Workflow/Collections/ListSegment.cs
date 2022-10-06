using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Utility.Collections
{
    public class ListSegment<T> : IList<T>, IList
    {
        private readonly IList<T> _innerList;
        private int _startIndex;
        private int _count;
        private UncertainBoolean _fixedSize;

        public ListSegment(IList<T> innerList, int startIndex, int count)
        {
            if (innerList == null)
                throw new ArgumentNullException(nameof(innerList));

            _innerList = innerList;

            SetScopeDirectly(startIndex, count);
        }

        protected virtual void SetScopeDirectly(int startIndex, int count)
        {
            Util.ValidateNamedRange(startIndex, count, _innerList.Count, indexName: nameof(startIndex), lengthName: nameof(_innerList.Count));

            _startIndex = startIndex;
            _count = count;
        }

        protected virtual bool GetIsFixedSize()
        {
            return _innerList is IList fixedList && fixedList.IsFixedSize;
        }

        public bool IsFixedSize
        {
            get { return (_fixedSize.IsUncertain ? _fixedSize = GetIsFixedSize() : _fixedSize).IsTrue; }
        }

        public void SetScope(int startIndex, int count)
        {
            CheckScopeFixed();

            SetScopeDirectly(startIndex, count);
        }

        public int StartIndex
        {
            get => _startIndex;
            set => SetScope(value, Count);
        }

        public int Count
        {
            get => _count;
            set => SetScope(StartIndex, value);
        }

        public void ExtendScope(int startIndexExtension, int countExtension)
        {
            SetScope(StartIndex - startIndexExtension, Count + startIndexExtension + countExtension);
        }

        public T this[int index]
        {
            get
            {
                Util.ValidateNamedIndex(index, Count, indexName: nameof(index), lengthName: nameof(Count));
                return _innerList[StartIndex + index];
            }
            set
            {
                CheckReadonly(true);
                Util.ValidateNamedIndex(index, Count, indexName: nameof(index), lengthName: nameof(Count));

                _innerList[StartIndex + index] = value;
            }
        }

        public virtual bool IsReadOnly => _innerList.IsReadOnly;

        public virtual bool IsScopeFixed
        {
            get { return false; }
        }

        public bool CanSet => _innerList is IList l && !l.IsReadOnly;

        public void Add(T item)
        {
            CheckReadonly(false);
            CheckFixedSize();

            _innerList.Insert(StartIndex + Count, item);
            SetScopeDirectly(StartIndex, Count + 1);
        }

        public void Clear()
        {
            CheckReadonly(true);

            if (IsFixedSize)
            {
                if (!(_innerList is T[] array))
                {
                    int c = StartIndex + Count;

                    for (int i = StartIndex; i < c; i++)
                        _innerList[i] = default;
                }
                else
                {
                    Array.Clear(array, StartIndex, Count);
                }
            }
            else
            {
                if (!(_innerList is List<T> l))
                {
                    for (int i = Count - 1; i >= 0; i--)
                        _innerList.RemoveAt(StartIndex + i);
                }
                else
                    l.RemoveRange(StartIndex, Count);

                SetScopeDirectly(StartIndex, 0);
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Util.ValidateNamedRange(array, arrayIndex, Count, indexName: nameof(arrayIndex));

            if (!(_innerList is T[] innerArray))
            {
                if (!(_innerList is List<T> list))
                {
                    int sind = StartIndex;
                    int eind = sind + Count;

                    for (int i = sind; i < eind; i++)
                        array[arrayIndex++] = _innerList[i];
                }
                else
                {
                    list.CopyTo(StartIndex, array, arrayIndex, Count);
                }
            }
            else
            {
                Array.Copy(innerArray, StartIndex, array, arrayIndex, Count);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            int c = Count;

            for (int i = 0; i < c; i++)
            {
                if (c != Count)
                    throw new InvalidOperationException("The enumeration scope has changed during iteration.");

                yield return this[i];
            }
        }

        public int IndexOf(T item)
        {
            int s = StartIndex;

            int absoluteIndex;

            if (!(_innerList is T[]array))
            {
                if (!(_innerList is List<T> list))
                {
                    EqualityComparer<T> c = EqualityComparer<T>.Default;

                    for (int i = 0; i < Count; i++)
                        if (c.Equals(_innerList[i + s], item))
                            return i;

                    return -1;
                }
                else
                {
                    absoluteIndex = list.IndexOf(item, s, Count);
                }
            }
            else
            {
                absoluteIndex =  Array.IndexOf(array, item, s, Count);
            }

            if (absoluteIndex < s || absoluteIndex >= s + Count)
                return -1;

            return absoluteIndex - s;
        }

        public void Insert(int index, T item)
        {
            CheckReadonly(false);
            CheckFixedSize();

            Util.ValidateNamedIndex(index, Count + 1, indexName: nameof(index), lengthName: nameof(Count));

            _innerList.Insert(index + StartIndex, item);
            SetScopeDirectly(StartIndex, Count + 1);
        }

        public bool Remove(T item)
        {
            CheckReadonly(false);
            CheckFixedSize();

            int i = IndexOf(item);

            if (i < 0)
                return false;

            _innerList.RemoveAt(i + StartIndex);
            SetScopeDirectly(StartIndex, Count - 1);

            return true;
        }

        public void RemoveAt(int index)
        {
            CheckReadonly(false);
            CheckFixedSize();

            Util.ValidateNamedIndex(index, Count, indexName: nameof(index), lengthName: nameof(Count));

            _innerList.RemoveAt(index + StartIndex);
            SetScopeDirectly(StartIndex, Count - 1);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void CheckReadonly(bool isSetOperation)
        {
            if (isSetOperation ? !CanSet : IsReadOnly)
                throw new InvalidOperationException("The current list is read-only.");
        }

        private void CheckScopeFixed()
        {
            if (IsScopeFixed)
                throw new InvalidOperationException("The current list scope is fixed.");
        }

        private void CheckFixedSize()
        {
            if (IsFixedSize)
                throw new InvalidOperationException("The current list has a fixed size.");
        }

        public static ListSegment<T> Restrict(IList<T> innerList, int startIndex, int count, bool readOnly, bool fixedScope, bool fixedSize)
        {
            if (innerList == null)
                throw new ArgumentNullException(nameof(innerList));

            Changes c = 0;

            if (readOnly)
                c |= Changes.ReadOnly;
            if (fixedScope)
                c |= Changes.FixedScope;
            if (fixedSize)
                c |= Changes.FixedSize;

            return new RestrictedListSegment(innerList, startIndex, count, c);
        }

        #region "IList"

        bool IList.IsReadOnly => IsReadOnly && !CanSet;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        int IList.Add(object value)
        {
            T v = (T)value;

            int index = Count;

            Add(v);

            return index;
        }

        bool IList.Contains(object value)
        {
            if (!(value is T))
                return false;

            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            if (!(value is T))
                return -1;

            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (!(array is T[] arr))
                throw new ArgumentException("One-dimensional zero-based array with element type " + typeof(T).Name + " expected.", nameof(array));

            Util.ValidateNamedRange(arr, index, Count);

            CopyTo((T[])array, index);
        }

        #endregion

        private sealed class RestrictedListSegment : ListSegment<T>
        {
            private readonly Changes _changes;

            public override bool IsReadOnly => (_changes & Changes.ReadOnly) == Changes.ReadOnly;
            public override bool IsScopeFixed => (_changes & Changes.FixedScope) == Changes.FixedScope;

            protected override bool GetIsFixedSize() => (_changes & Changes.FixedSize) == Changes.FixedSize;

            public RestrictedListSegment(IList<T> innerList, int startIndex, int count, Changes changes) : base(innerList, startIndex, count)
            {
                _changes = changes;
            }
        }

        [Flags]
        private enum Changes
        {
            ReadOnly = 1,
            FixedSize = 2,
            FixedScope = 4
        }
    }
}
