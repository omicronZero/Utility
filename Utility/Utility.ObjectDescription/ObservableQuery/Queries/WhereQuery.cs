using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;

namespace Utility.ObjectDescription.ObservableQuery.Queries
{
    public class WhereQuery<TSource> : ObservationListQuery<TSource, TSource>
    {
        private readonly List<int> _indexMapping;
        private readonly Func<TSource, bool> _predicate;

        public WhereQuery(IObservationListQuery<TSource> underlyingQuery, Func<TSource, bool> predicate)
            : base(underlyingQuery)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            _indexMapping = new List<int>();
            _predicate = predicate;
            ResetItems();
        }

        public override IEnumerable<TSource> GetItems()
        {
            int i = 0;
            foreach (TSource s in UnderlyingInstance.GetItems())
            {
                if (_indexMapping[i++] >= 0)
                    yield return s;
            }
        }

        protected override void InsertItems(int previousIndex, IList<TSource> items)
        {
            int c = 0;
            List<TSource> s = null;

            foreach (TSource item in items)
            {
                bool contained = _predicate(item);
                //TODO: take contained and c > 0 into consideration to prevent unnecessary instantiation
                if (!contained)
                {
                    if (s != null)
                        s = new List<TSource>(items.Take(c));
                }
                else if (s != null)
                    s.Add(item);

                c++;
            }

            if (c > 0 && (s == null || s.Count > 0))
            {
                int baseIndex = _indexMapping[previousIndex];

                for (int i = previousIndex; i < _indexMapping.Count; i++)
                {
                    _indexMapping[i] += c;
                }
                //TODO: test performance
                _indexMapping.InsertRange(previousIndex, Enumerable.Range(baseIndex, c));

                OnCollectionChanged(ObservableCollectionChangedEventArgs<TSource>.InsertedRange(s ?? items, baseIndex));
            }
        }

        protected override void MoveItems(int newPreviousIndex, IList<TSource> items, int oldPreviousIndex)
        {
            //TODO: make more performant
            RemoveItems(oldPreviousIndex, items);
            InsertItems(newPreviousIndex, items);
        }

        protected override void RemoveItems(int previousIndex, IList<TSource> items)
        {
            int c = 0;
            int srcc = items.Count;

            int baseIndex = _indexMapping[previousIndex];
            List<TSource> s = null;

            for (int i = 0; i < srcc; i++)
            {
                if (_indexMapping[previousIndex + i] >= 0)
                {
                    c++;

                    if (s != null)
                        s.Add(items[i]);
                }
            }

            if (c > 0)
            {
                _indexMapping.RemoveRange(previousIndex, c);

                for (int i = previousIndex; i < _indexMapping.Count; i++)
                    _indexMapping[i] -= c;

                OnCollectionChanged(ObservableCollectionChangedEventArgs<TSource>.RemovedRange(s, baseIndex));
            }
        }

        protected override void ResetItems()
        {
            _indexMapping.Clear();

            int c = 0;

            foreach (TSource s in UnderlyingInstance.GetItems())
            {
                _indexMapping.Add(_predicate(s) ? c++ : -1);
            }
        }

        protected override void SetItems(int previousIndex, IList<TSource> newItems, IList<TSource> oldItems)
        {
            RemoveItems(previousIndex, oldItems);
            InsertItems(previousIndex, newItems);
        }
    }
}
