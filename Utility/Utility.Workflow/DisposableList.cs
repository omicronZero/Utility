using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility.Collections;

namespace Utility
{
    //TODO: test
    public class DisposableList<T> : ICollection<T>
    {
        private Entry _first;
        private Entry _last;

        public int Count { get; private set; }

        public object SyncObject { get; }

        public DisposableList()
            : this(false)
        { }

        public DisposableList(bool synchronized)
        {
            if (synchronized)
            {
                SyncObject = new object();
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        public IDisposable Add(T item)
        {
            return Add(item, null);
        }

        public IDisposable Add(T item, Action<T> disposeHandler)
        {
            return Util.CallSynchronized(SyncObject, this, item,
                (_this, it) =>
                {
                    _this.Count++;
                    Entry e = _this._last = new Entry(_this, _this._last, it, disposeHandler);

                    if (_first == null)
                        _first = e;

                    return e;
                });
        }

        void ICollection<T>.Add(T item)
        {
            Add(item, null);
        }

        public void Clear()
        {
            Util.CallSynchronized(SyncObject, () =>
            {
                for (Entry e = _first; e != null; e = e.Next)
                    e.Dispose();
            });
        }

        public bool Contains(T item)
        {
            return Util.CallSynchronized(SyncObject, _first, item, (_items, it) =>
            {
                var c = EqualityComparer<T>.Default;

                for (Entry e = _first; e != null; e = e.Next)
                    if (c.Equals(e.Value, it))
                        return true;

                return false;
            });
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Util.CallSynchronized(SyncObject, this, array, arrayIndex,
                (_this, arr, arrInd) => SelectorCollection.SelectCollection(_this, (t) => t).CopyTo(array, arrayIndex));
        }

        public T GetItem(IDisposable disposable)
        {
            if (disposable == null)
                throw new ArgumentNullException(nameof(disposable));

            var e = disposable as Entry;

            if (e == null)
                throw new ArgumentException($"Disposable does not correspond to an instance of { nameof(DisposableList<T>)}.", nameof(disposable));

            return Util.CallSynchronized(SyncObject, disposable, e, GetItemCore);
        }

        private T GetItemCore(IDisposable disposable, Entry e)
        {
            if (e.List == null)
                throw new ObjectDisposedException(nameof(disposable));

            if (e.List != this)
                throw new ArgumentException("Disposable does not correspond to the current instance.", nameof(disposable));

            return e.Value;
        }

        public IEnumerable<KeyValuePair<T, IDisposable>> GetHandleIterator()
        {
            for (Entry e = _first; e != null; e = e.Next)
            {
                yield return new KeyValuePair<T, IDisposable>(e.Value, e);
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_first);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<T>.Remove(T item)
        {
            return Util.CallSynchronized(SyncObject, this, item, (_this, it) =>
            {
                EqualityComparer<T> c = EqualityComparer<T>.Default;

                for (Entry e = _first; e != null; e = e.Next)
                    if (c.Equals(e.Value, it))
                    {
                        e.Dispose();
                        return true;
                    }

                return false;
            });
        }

        internal sealed class Entry : IDisposable
        {
            private Action<T> _disposeHandler;

            private T _value;
            public DisposableList<T> List { get; private set; }
            public Entry Next { get; private set; }
            public Entry Previous { get; private set; }

            public Entry(DisposableList<T> list, Entry previous, T value, Action<T> disposeHandler)
            {
                if (list == null)
                    throw new ArgumentNullException(nameof(list));

                if (previous != null)
                {
                    previous.Next = this;
                    Previous = previous;
                }

                List = list;
                Value = value;
                _disposeHandler = disposeHandler;
            }

            public T Value
            {
                get
                {
                    if (List == null)
                        throw new ObjectDisposedException(this.GetType().Name);

                    return _value;
                }
                private set => _value = value;
            }

            public void Dispose()
            {
                if (List != null)
                {
                    T value = Value;

                    Util.CallSynchronized(List.SyncObject, () =>
                    {
                        List.Count--;

                        if (Previous != null)
                            Previous.Next = Next;

                        if (Next != null)
                            Next.Previous = Previous;

                        if (List._first == this)
                            List._first = Next;
                        if (List._last == this)
                            List._last = Previous;

                        List = null;
                    });


                    _disposeHandler?.Invoke(value);
                    _disposeHandler = null;

                    //To allow the DisposableList to iterate through the items ignoring removed entries, Next is not set to a new value

                    Value = default;
                }
            }
        }

        public struct Enumerator : IEnumerator<T>
        {
            private Entry _first;
            private Entry _current;
            private bool _initialized;
            private T _currentValue;

            internal Enumerator(Entry first)
            {
                _first = first;
                _current = null;
                _initialized = false;
                _currentValue = default;
            }

            public T Current
            {
                get
                {
                    if (!_initialized)
                        throw new InvalidOperationException("The enumerator has not been initialized.");

                    if (_current == null)
                        throw new InvalidOperationException("The enumerator has ended.");

                    return _currentValue;
                }
            }

            object IEnumerator.Current => Current;

            void IDisposable.Dispose()
            { }

            public bool MoveNext()
            {
                if (_initialized && _current == null)
                    return false;

                if (!_initialized)
                {
                    _initialized = true;
                    _current = _first;
                }
                else
                {
                    do
                    {
                        _current = _current.Next;
                    } while (_current != null && _current.List == null);
                }

                if (_current == null)
                    return false;

                _currentValue = _current.Value;
                return true;
            }

            public void Reset()
            {
                _current = null;
                _initialized = false;
            }
        }
    }
}
