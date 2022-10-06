using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    //TODO: replace _buffered bit array with more efficient implementation
    public class VirtualizedList<T> : IList<T>
    {
        private readonly Func<int, T> _allocator;
        private readonly Action<int, T> _releaser;
        private readonly Func<int> _countEvaluator;

        private T[] _buffer;
        private BitArray _buffered;

        public int RangeStart { get; private set; }
        public int RangeCount { get; private set; }

        private int _count;

        public VirtualizedList(Func<int> countEvaluator, Func<int, T> allocator, Action<T> releaser)
            : this(countEvaluator, allocator, releaser == null ? (Action<int, T>)null : (i, t) => releaser(t))
        { }

        public VirtualizedList(Func<int> countEvaluator, Func<int, T> allocator, Action<int, T> releaser)
        {
            if (countEvaluator == null)
                throw new ArgumentNullException(nameof(countEvaluator));

            if (allocator == null)
                throw new ArgumentNullException(nameof(allocator));

            _countEvaluator = countEvaluator;
            _allocator = allocator;
            _releaser = releaser;
        }

        public void SetBufferedRange(int startIndex, int count)
        {
            Util.ValidateNamedRange(startIndex, count, Count, indexName: nameof(startIndex), lengthName: nameof(Count));

            if (startIndex != RangeStart)
            {
                //buffer may need resizing, items must be moved
                T[] b = _buffer;

                if (count != RangeCount)
                    ReallocateBuffer(count);

                //if the new range overlaps with the old one, copy items and clear relevant buffer range
                //otherwise clear entire buffer
                /*
                 * Source: {_rangeStart, ..., _rangeStart + _rangeCount}
                 * Destination: {startIndex, ..., startIndex + count}
                 * 
                 * ==> new index of item at _rangeStart is
                 */
                int overlapStart = Math.Max(startIndex, RangeStart);
                int overlapCount = Math.Min(RangeStart + RangeCount, startIndex + count) - overlapStart;

                if (overlapCount > 0)
                {
                    Array.Copy(b, RangeStart, _buffer, overlapStart, overlapCount);

                    if (overlapStart > 0)
                    {
                        for (int i = 0; i < overlapStart; i++)
                        {
                            //WARNING: critical test
                            if (_buffered[i])
                                _releaser?.Invoke(i, b[i - RangeStart]);

                            _buffered[i] = false;
                        }

                        Array.Clear(_buffer, 0, overlapStart);
                    }

                    //WARNING: critical test
                    if (overlapStart + overlapCount < _buffer.Length)
                    {
                        for (int i = overlapStart + overlapCount; i < _buffer.Length - (overlapStart + overlapCount); i++)
                        {
                            if (_buffered[i])
                                _releaser?.Invoke(i, b[i - RangeStart]);

                            _buffered[i] = false;
                        }

                        Array.Clear(_buffer, overlapStart + overlapCount, _buffer.Length - (overlapStart + overlapCount));
                    }
                }
                else
                {
                    for (int i = 0; i < b.Length; i++)
                        if (_buffered[i])
                            _releaser?.Invoke(i, b[i + RangeStart]);

                    Array.Clear(_buffer, 0, _buffer.Length);
                    _buffered.SetAll(false);
                }
            }
            else
            {
                //buffer may need resizing but items do not need to be moved
                ReallocateBuffer(count);
            }

            RangeStart = startIndex;
            RangeCount = count;
        }

        public void UpdateLength()
        {
            //TODO: optimize
            //limit range of virtualized list to size

            int newCount = _countEvaluator();

            if (newCount >= RangeStart + RangeCount)
                return;

            int newRangeStart = newCount - RangeCount;
            int newRangeCount = Math.Min(RangeCount, _buffer.Length - Math.Max(newRangeStart, 0));

            if (newRangeStart < 0)
            {
                //if new count affects range (by cutting off the end), move range towards the start of the buffer
                SetBufferedRange(0, newRangeCount);
            }
            else if (newRangeStart != RangeStart)
                SetBufferedRange(newRangeStart, newRangeCount);

            _count = newCount;

            //if necessary, adjust buffer size (if previousRangeCount was bigger than newCount)
            ReallocateBuffer(newCount);
        }

        private void ReallocateBuffer(int length)
        {
            uint ul = 1;

            while (ul <= length)
                ul <<= 1;

            if (ul > int.MaxValue)
                ul = int.MaxValue;

            int l = unchecked((int)ul);

            if (l != _buffer.Length)
            {
                //reallocation is necessary
                Array.Resize(ref _buffer, l);

                var b = new BitArray(l);

                //TODO: replace BitArray with custom implementation, improve efficiency
                for (int i = 0; i < Math.Min(l, _buffered.Length); i++)
                    b[i] = _buffered[i];

                _buffered = b;
            }
        }

        public void Reset()
        {
            _buffered.SetAll(false);
            Array.Clear(_buffer, 0, _buffer.Length);
        }

        public T this[int index]
        {
            get
            {
                Util.ValidateNamedIndex(index, Count, lengthName: nameof(Count));

                if (index < RangeStart || index >= RangeCount)
                    return _allocator(index);

                index -= RangeStart;

                if (_buffered[index])
                    return _buffer[index];

                T v = _allocator(index + RangeStart);

                _buffer[index] = v;

                return v;
            }
        }

        T IList<T>.this[int index]
        {
            get => this[index];
            set => ThrowReadonly();
        }

        private static void ThrowReadonly()
        {
            throw new NotSupportedException("List is read-only.");
        }

        public int Count => _count;

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Util.ValidateNamedIndex(array, arrayIndex, indexName: nameof(arrayIndex));

            foreach (T v in this)
                array[arrayIndex++] = v;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < RangeCount; i++)
                if (_buffered[i] && EqualityComparer<T>.Default.Equals(_buffer[i], item))
                    return i + RangeStart;

            for (int i = 0; i < RangeStart; i++)
                if (EqualityComparer<T>.Default.Equals(this[i], item))
                    return i;

            for (int i = RangeStart + RangeCount; i < Count; i++)
                if (EqualityComparer<T>.Default.Equals(this[i], item))
                    return i;

            return -1;
        }

        bool ICollection<T>.IsReadOnly => true;

        void ICollection<T>.Add(T item)
        {
            ThrowReadonly();
        }

        void ICollection<T>.Clear()
        {
            ThrowReadonly();
        }

        void IList<T>.Insert(int index, T item)
        {
            ThrowReadonly();
        }

        bool ICollection<T>.Remove(T item)
        {
            ThrowReadonly();
            //never reached
            return false;
        }

        void IList<T>.RemoveAt(int index)
        {
            ThrowReadonly();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
