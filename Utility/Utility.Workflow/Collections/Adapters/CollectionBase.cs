using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility.Collections.Tools;

namespace Utility.Workflow.Collections.Adapters
{
    public abstract class CollectionBase<T> : ICollection, ICollection<T>, IReadOnlyCollection<T>
    {
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }

        public abstract void Add(T item);
        public abstract void Clear();
        public abstract bool Contains(T item);
        public abstract IEnumerator<T> GetEnumerator();
        public abstract bool Remove(T item);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            CollectionExceptions.CheckCopyTo(Count, array, arrayIndex);

            foreach (T v in this)
                array[arrayIndex++] = v;
        }

        protected virtual object SyncRoot => null;

        bool ICollection.IsSynchronized => SyncRoot != null;

        object ICollection.SyncRoot => SyncRoot;

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (!(array is T[] castArray))
                throw new ArgumentException($"One-dimensional zero-based array of type {typeof(T).FullName} expected.", nameof(array));

            CopyTo(castArray, index);
        }

        protected static Exception FixedSizeException()
        {
            return new InvalidOperationException("The collection has a fixed size.");
        }
    }
}
