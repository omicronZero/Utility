using System;
using System.Collections;
using System.Collections.Generic;
using Utility.Workflow.Collections.Adapters;

namespace Utility.Tests.Collection
{
    public class FixedSizeMock : IList
    {
        private readonly IList _underlyingList;

        public FixedSizeMock(IList underlyingList)
        {
            _underlyingList = underlyingList;
        }

        public object this[int index] { get => _underlyingList[index]; set => _underlyingList[index] = value; }

        public bool IsFixedSize => true;

        public bool IsReadOnly => _underlyingList.IsReadOnly;

        public int Count => _underlyingList.Count;

        public bool IsSynchronized => _underlyingList.IsSynchronized;

        public object SyncRoot => _underlyingList.SyncRoot;

        public int Add(object value)
        {
            return _underlyingList.Add(value);
        }

        public void Clear()
        {
            _underlyingList.Clear();
        }

        public bool Contains(object value)
        {
            return _underlyingList.Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            _underlyingList.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return _underlyingList.GetEnumerator();
        }

        public int IndexOf(object value)
        {
            return _underlyingList.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            _underlyingList.Insert(index, value);
        }

        public void Remove(object value)
        {
            _underlyingList.Remove(value);
        }

        public void RemoveAt(int index)
        {
            _underlyingList.RemoveAt(index);
        }
    }

    public class FixedSizeMock<T> : ListBase<T>
    {
        private readonly T[] _array;

        public bool DenyItemChanges { get; }

        public FixedSizeMock(T[] array, bool denyItemChanges)
        {
            _array = array;
            DenyItemChanges = denyItemChanges;
        }

        public override T this[int index]
        {
            get => _array[index];
            set
            {
                if (DenyItemChanges)
                    throw new InvalidOperationException("The collection is read-only.");

                _array[index] = value;
            }
        }

        public override bool IsReadOnly => true;
        
        public override int Count => _array.Length;

        public override void Clear()
        {
            throw FixedSizeException();
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        protected override bool IsFixedSize => true;

        protected override bool IsSetterReadOnly => DenyItemChanges;

        public override int IndexOf(T item)
        {
            return Array.IndexOf(_array, item);
        }

        public override void Insert(int index, T item)
        {
            throw FixedSizeException();
        }

        public override void RemoveAt(int index)
        {
            throw FixedSizeException();
        }
    }
}
