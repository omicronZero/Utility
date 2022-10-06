using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace Utility
{
    [Serializable]
    public struct ValueStack<T> : ISerializable, IEnumerable<T>
    {
        private object _parent;
        private T _value;

        private ValueStack(SerializationInfo info, StreamingContext context)
            : this()
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            T[] v = info.GetValue<T[]>("Stack");

            if (v == null)
                throw new ArgumentException("The Stack entry in the serialized data must not be null.", nameof(info));

            for (int i = v.Length - 1; i >= 0; i--)
            {
                Push(v[i]);
            }
        }

        public void Push(T value)
        {
            _parent = this;
            _value = value;
        }

        public bool IsEmpty => _parent == null;

        public T Pop()
        {
            T v = Current;

            this = (ValueStack<T>)_parent;

            return v;
        }

        public void Skip()
        {
            if (!IsEmpty)
            {
                this = (ValueStack<T>)_parent;
            }
        }

        public bool Peek(out T value)
        {
            if (_parent == null)
            {
                value = default(T);
                return false;
            }

            value = _value;
            return true;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue("Stack", this.ToArray());
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T Current
        {
            get
            {
                T v;

                if (!Peek(out v))
                    throw new InvalidOperationException("The stack is empty.");

                return _value;
            }
        }

        public static ValueStack<T> operator +(ValueStack<T> left, T right)
        {
            left.Push(right);
            return left;
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly ValueStack<T> _top;
            private ValueStack<T> _current;
            private bool _initialized;

            internal Enumerator(ValueStack<T> stack)
            {
                _top = stack;
                _current = stack;
                _initialized = false;
            }

            public T Current => _current.Current;

            object IEnumerator.Current => Current;

            void IDisposable.Dispose()
            { }

            public bool MoveNext()
            {
                if (!_initialized)
                {
                    _initialized = true;
                    _current = _top;
                }
                else
                {
                    _current.Pop();
                }

                return !_current.IsEmpty;
            }

            public void Reset()
            {
                _current = _top;
            }
        }
    }
}
