using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility
{
    [Serializable]
    public sealed class ReadonlyArray<T> : IList<T>, IEquatable<ReadonlyArray<T>>, ISerializable
    {
        public static ReadonlyArray<T> Empty { get; } = new ReadonlyArray<T>(0, null, false, false);

        private readonly Func<int, T> _evaluator;
        private readonly T[] _entries;
        private readonly BitArray _evaluated;

        public ReadonlyArray(int length, Func<int, T> evaluator)
            : this(length, evaluator ?? throw new ArgumentNullException(nameof(evaluator)), true, true)
        { }

        private ReadonlyArray(int length, Func<int, T> evaluator, bool createEntries, bool createEvaluated)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Non-negative length expected.");

            if (createEntries)
                _entries = new T[length];

            if (createEntries && createEvaluated)
                _evaluated = new BitArray(length);

            _evaluator = evaluator;
        }

        private ReadonlyArray(T[] filledArray)
        {
            if (filledArray == null)
                throw new ArgumentNullException(nameof(filledArray));

            _entries = filledArray;
        }

        private ReadonlyArray(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            var entries = (T[])info.GetValue("Entries", typeof(T[]));

            _entries = entries;
        }

        private void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            T[] entries = ToArray();

            info.AddValue("Entries", entries);
        }

        public T[] ToArray()
        {
            int len = Length;

            var array = new T[len];

            for (int i = 0; i < len; i++)
                array[i] = this[i];

            return array;
        }

        public int Length
        {
            get { return _entries?.Length ?? 0; }
        }

        public T this[int index]
        {
            get
            {
                Util.ValidateIndex(index, Length);

                T v;

                /*three cases:
                 * _entries != null && evaluated != null:
                 *      array has been created regularly and holds non-initialized entries
                 *      
                 * _entries != null && evaluated == null:
                 *      All entries are evaluated (deserialized arrays make use of this)
                 *      
                 * _entries == null && evaluated == null:
                 *      The evaluator is used to retrieve the entries (Range function makes use of this)
                 */

                if (_entries != null)
                    if (_evaluated == null || _evaluated[index])
                        v = _entries[index];
                    else
                    {
                        v = _evaluator(index);
                        _entries[index] = v;
                        _evaluated[index] = true;
                    }
                else
                    v = _evaluator(index);

                return v;
            }
        }

        public bool IsInitialized(int index) => _evaluated?[index] ?? true;

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Util.ValidateNamedIndex(array, arrayIndex, indexName: nameof(arrayIndex));

            long ind = arrayIndex;

            foreach (T v in this)
                array[ind++] = v;
        }

        public int IndexOf(T item)
        {
            int len = Length;
            var cmp = EqualityComparer<T>.Default;

            for (int i = 0; i < len; i++)
                if (cmp.Equals(this[i], item))
                    return i;

            return -1;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        private void ThrowReadonly()
        {
            throw new NotSupportedException("The list is read-only.");
        }

        T IList<T>.this[int index]
        {
            get => this[index];
            set => ThrowReadonly();
        }

        int ICollection<T>.Count => Length;

        bool ICollection<T>.IsReadOnly => true;

        void ICollection<T>.Add(T item)
        {
            ThrowReadonly();
        }

        void ICollection<T>.Clear()
        {
            ThrowReadonly();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as ReadonlyArray<T>;

            if (other == null)
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            int v = 0;
            int len = Length;

            for (int i = 0; i < len; i++)
                v ^= this[i]?.GetHashCode() ?? 0;

            return v;
        }

        public bool Equals(ReadonlyArray<T> other)
        {
            if (other == null)
                return false;

            int len = Length;

            if (len != other.Length)
                return false;

            var cmp = EqualityComparer<T>.Default;

            for (int i = 0; i < len; i++)
                if (!cmp.Equals(this[i], other[i]))
                    return false;

            return true;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        public static implicit operator ReadonlyArray<T>(T[] array)
        {
            if (array == null)
                return null;

            return new ReadonlyArray<T>(array.Length, (i) => array[i]);
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly ReadonlyArray<T> _array;
            private int _index;

            internal Enumerator(ReadonlyArray<T> array)
            {
                _array = array;
                _index = -1;
            }

            public T Current
            {
                get
                {
                    ThrowInvalid();

                    if (_index < 0)
                        throw new InvalidOperationException("Enumerator has not been initialized.");

                    if (_index >= _array.Length)
                        throw new InvalidOperationException("Enumerator has already ended.");

                    return _array[_index];
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                ThrowInvalid();
            }

            public bool MoveNext()
            {
                ThrowInvalid();
                if (_index >= _array.Length)
                    return false;

                return ++_index < _array.Length;
            }

            public void Reset()
            {
                ThrowInvalid();
            }

            private void ThrowInvalid()
            {
                if (_array == null)
                    throw new InvalidOperationException("Enumerator is not valid.");
            }
        }
    }
}
