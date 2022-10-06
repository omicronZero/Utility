using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Collections.Tools;

namespace Utility.Collections
{
    public class OverflowingBuffer<T> : IList<T>, IReadOnlyList<T>, IList
    {
        private readonly T[] _items;
        private int _currentWritePosition;

        public int Count { get; private set; }

        public OverflowingBuffer(int maxItemsStored)
        {
            if (maxItemsStored < 0)
                throw new ArgumentOutOfRangeException(nameof(maxItemsStored), "Non-negative number of maximum items stored expected.");

            _items = new T[maxItemsStored];
        }

        internal T[] InternalBuffer => _items;

        internal int InternalWritePosition => _currentWritePosition;

        public int Capacity => _items.Length;

        public void Reset()
        {
            Array.Clear(_items, 0, _items.Length);
            Count = 0;
            _currentWritePosition = 0;
        }

        public void Dequeue(int count)
        {
            if (count > Count)
                throw new ArgumentException("The indicated amount of items exceeds the buffer's currently stored amount of items.");

            Count -= count;
        }

        public void Dequeue(int count, bool clear)
        {
            Dequeue(count);

            if (clear)
            {
                int prevDelimiter = _currentWritePosition - Count - count;
                int len = _items.Length;

                if (prevDelimiter < 0)
                    prevDelimiter += len;

                if (prevDelimiter + count > len)
                {
                    Array.Clear(_items, prevDelimiter, len - prevDelimiter);
                    count -= len - prevDelimiter;
                    prevDelimiter += len - prevDelimiter;
                }

                Array.Clear(_items, prevDelimiter, count);
            }
        }

        public void Push(T[] items, int index, int count)
        {
            Util.ValidateNamedRange(items, index, count, arrayName: nameof(items));

            if (count > _items.Length) //total buffer size exceeded, take tail of items
            {
                Array.Copy(items, items.Length - _items.Length, _items, 0, _items.Length);
                _currentWritePosition = 0;
                Count = _items.Length;
            }
            else
            {
                int c = Math.Min(_items.Length - _currentWritePosition, count);
                Array.Copy(items, index, _items, _currentWritePosition, c);

                if (count - c > 0)
                {
                    Array.Copy(items, index + c, _items, 0, count - c);
                    _currentWritePosition = count - c;
                }
                else
                    _currentWritePosition = (_currentWritePosition + c) % _items.Length;

                Count = Math.Min(Count + count, _items.Length);
            }
        }

        public void Push(T[] items)
        {
            Push(items, 0, items?.Length ?? 0);
        }

        public void Push(T item)
        {
            if (_items.Length == 0)
                return;

            _items[_currentWritePosition] = item;
            _currentWritePosition = (_currentWritePosition + 1) % _items.Length;
            Count = Math.Min(Count + 1, _items.Length);
        }

        public int IndexOf(T item)
        {
            int c = Count;
            int d = _currentWritePosition;

            int len = _items.Length;

            int firstIndex = _currentWritePosition - c;

            if (firstIndex < 0)
                firstIndex += len;

            int ind = Array.IndexOf(_items, item, firstIndex, Math.Min(len - firstIndex, c));

            if (ind >= 0)
                return ind - firstIndex;

            ind = Array.IndexOf(_items, item, 0, _currentWritePosition);

            if (ind < 0)
                return -1;

            return ind + (len - firstIndex);
        }

        bool ICollection<T>.IsReadOnly => true;

        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw CollectionExceptions.ReadOnlyException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        void ICollection<T>.Add(T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        void ICollection<T>.Clear()
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            SelectorList.SelectList(this, (s) => s).CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            int c = Count;
            int d = _currentWritePosition;


            int len = _items.Length;

            int cind = _currentWritePosition - c;

            if (cind < 0)
                cind += len;

            for (int i = 0; i < c; i++)
            {
                if (c != Count || _currentWritePosition != d)
                    throw new InvalidOperationException("The underlying buffer has changed.");

                yield return _items[cind];

                if (++cind == len)
                    cind = 0;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int GetCurrentReadOffset()
        {
            return (_currentWritePosition + _items.Length - Count) % _items.Length;
        }

        public T this[int index]
        {
            get
            {
                Util.ValidateIndex(index, Count);

                return _items[(index + GetCurrentReadOffset()) % _items.Length];
            }
        }

        #region IList

        private ListHelper<T, OverflowingBuffer<T>> ListHelper => new ListHelper<T, OverflowingBuffer<T>>(this);

        object IList.this[int index]
        {
            get => ((IList)ListHelper)[index];
            set => ((IList)ListHelper)[index] = value;
        }

        bool IList.IsFixedSize => ((IList)ListHelper).IsFixedSize;

        bool IList.IsReadOnly => ((IList)ListHelper).IsReadOnly;

        bool ICollection.IsSynchronized => ((IList)ListHelper).IsSynchronized;

        object ICollection.SyncRoot => ((IList)ListHelper).SyncRoot;

        int IList.Add(object value)
        {
            return ((IList)ListHelper).Add(value);
        }

        void IList.Clear()
        {
            ((IList)ListHelper).Clear();
        }

        bool IList.Contains(object value)
        {
            return ((IList)ListHelper).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)ListHelper).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            ((IList)ListHelper).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            ((IList)ListHelper).Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            ((IList)ListHelper).RemoveAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)ListHelper).CopyTo(array, index);
        }

        #endregion
    }
}
