using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility.Collections.Tools;

namespace Utility.Collections
{
    public sealed class ListAccessor<T> : ListBase<T>
    {
        private readonly Func<int, T> _indexer;
        private readonly Func<int> _countEvaluator;

        public ListAccessor(Func<int, T> indexer, Func<int> countEvaluator, bool checkBounds)
        {
            if (indexer == null)
                throw new ArgumentNullException(nameof(indexer));

            if (countEvaluator == null)
                throw new ArgumentNullException(nameof(countEvaluator));

            if (checkBounds)
            {
                Func<int, T> oldIndexer = indexer;
                indexer = (i) =>
                {
                    Util.ValidateIndex(i, countEvaluator());

                    return oldIndexer(i);
                };
            }

            _indexer = indexer;
            _countEvaluator = countEvaluator;
        }

        public override T this[int index]
        {
            get => _indexer(index);
            set => throw CollectionExceptions.ReadOnlyException();
        }

        public override int Count => _countEvaluator();

        public override bool IsReadOnly => true;

        public override bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            SelectorCollection.SelectCollection(this, (s) => s).CopyTo(array, arrayIndex);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            int c = _countEvaluator();

            for (int i = 0; i < c; i++)
                yield return _indexer(i);
        }

        public override int IndexOf(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            int ind = 0;

            foreach (T c in this)
            {
                if (comparer.Equals(c, item))
                    return ind;

                ind++;
            }

            return -1;
        }

        public override void Add(T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public override void Clear()
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public override void Insert(int index, T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public override bool Remove(T item)
        {
            throw CollectionExceptions.ReadOnlyException();
        }

        public override void RemoveAt(int index)
        {
            throw CollectionExceptions.ReadOnlyException();
        }
    }
}
