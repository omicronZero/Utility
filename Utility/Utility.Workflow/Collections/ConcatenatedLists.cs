using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Utility.Collections.Tools;

namespace Utility.Collections
{
    [Serializable]
    public struct ConcatenatedLists<T> : IList<T>, IReadOnlyList<T>, ISerializable
    {
        private readonly IEnumerable<IList<T>> _lists;

        public ConcatenatedLists(params IList<T>[] lists)
            : this((IEnumerable<IList<T>>)(lists ?? throw new ArgumentNullException(nameof(lists))))
        { }

        public ConcatenatedLists(IEnumerable<IList<T>> listsEnumerable)
        {
            if (listsEnumerable == null)
                throw new ArgumentNullException(nameof(listsEnumerable));

            _lists = listsEnumerable;
        }

        private ConcatenatedLists(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            _lists = info.GetValue<IEnumerable<IList<T>>>("Lists");
        }

        public T this[int index]
        {
            get
            {
                if (_lists == null)
                    throw new NullReferenceException();

                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), index, "Non-negative index expected.");

                foreach (IList<T> l in _lists)
                {
                    int c = l.Count;

                    if (index < c)
                        return l[index];

                    index -= c;
                }

                throw new ArgumentOutOfRangeException(nameof(index), index, "Index does not fall into the range of the list.");
            }
        }

        T IList<T>.this[int index]
        {
            get => this[index];
            set
            {
                if (_lists == null)
                    throw new NullReferenceException();

                throw CollectionExceptions.ReadOnlyException();
            }
        }

        public int Count
        {
            get
            {
                if (_lists == null)
                    throw new NullReferenceException();

                int tc = 0;

                foreach (IList<T> v in _lists)
                    tc += v.Count;

                return tc;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                if (_lists == null)
                    throw new NullReferenceException();

                return true;
            }
        }

        public bool Contains(T item)
        {
            if (_lists == null)
                throw new NullReferenceException();

            foreach (IList<T> v in _lists)
            {
                if (v.Contains(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_lists == null)
                throw new NullReferenceException();

            Util.ValidateNamedRange(array, arrayIndex, array.LongLength, indexName: nameof(arrayIndex));

            foreach (IList<T> v in _lists)
            {
                v.CopyTo(array, arrayIndex);
                arrayIndex += v.Count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_lists == null)
                throw new NullReferenceException();

            return _lists.SelectMany((o) => o).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            int offset = 0;

            foreach (IList<T> l in _lists)
            {
                int cind = l.IndexOf(item);

                if (cind >= 0)
                    return offset + cind;

                offset += l.Count;
            }

            return offset;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_lists == null)
                throw new NullReferenceException();

            return GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            if (_lists == null)
                throw new NullReferenceException();

            throw CollectionExceptions.ReadOnlyException();
        }

        void ICollection<T>.Clear()
        {
            if (_lists == null)
                throw new NullReferenceException();

            throw CollectionExceptions.ReadOnlyException();
        }

        void IList<T>.Insert(int index, T item)
        {
            if (_lists == null)
                throw new NullReferenceException();

            throw CollectionExceptions.ReadOnlyException();
        }

        bool ICollection<T>.Remove(T item)
        {
            if (_lists == null)
                throw new NullReferenceException();

            throw CollectionExceptions.ReadOnlyException();
        }

        void IList<T>.RemoveAt(int index)
        {
            if (_lists == null)
                throw new NullReferenceException();

            throw CollectionExceptions.ReadOnlyException();
        }

        private void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (_lists == null)
                throw new NullReferenceException();

            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue("Lists", _lists);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (_lists == null)
                throw new NullReferenceException();

            GetObjectData(info, context);
        }
    }
}
