using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Utility.Collections;

namespace Utility.ObjectDescription.ObservableQuery.Queries
{
    public class SelectorQuery<TSource, TResult> : ObservationListQuery<TSource, TResult>
    {
        private readonly Func<TSource, TResult> _selector;
        private readonly Dictionary<TSource, (TResult Result, int ReferenceCount)> _mapping;

        public SelectorQuery(IObservationListQuery<TSource> underlyingQuery, Func<TSource, TResult> selector, bool convertOnce)
            : base(underlyingQuery)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            _selector = selector;

            if (convertOnce && !typeof(TSource).IsValueType)
                _mapping = new Dictionary<TSource, (TResult, int)>(ReferenceComparer<TSource>.Default);

            ResetItems();
        }

        protected override void InsertItems(int previousIndex, IList<TSource> items)
        {
            OnCollectionChanged(ObservableCollectionChangedEventArgs<TResult>.InsertedRange(items.SelectList(MapItem), previousIndex));

            foreach (TSource src in items)
            {
                Increment(src);
            }
        }

        protected override void MoveItems(int newPreviousIndex, IList<TSource> items, int oldPreviousIndex)
        {
            OnCollectionChanged(ObservableCollectionChangedEventArgs<TResult>.MovedRange(items.SelectList(MapItem), newPreviousIndex, oldPreviousIndex));
        }

        protected override void RemoveItems(int previousIndex, IList<TSource> items)
        {
            OnCollectionChanged(ObservableCollectionChangedEventArgs<TResult>.RemovedRange(items.SelectList(MapItem), previousIndex));

            foreach (TSource src in items)
            {
                Decrement(src);
            }
        }

        protected override void ResetItems()
        {
            OnCollectionChanged(ObservableCollectionChangedEventArgs<TResult>.Reset());

            _mapping.Clear();

            foreach (TSource src in UnderlyingInstance.GetItems())
            {
                Increment(src);
            }
        }

        protected override void SetItems(int previousIndex, IList<TSource> newItems, IList<TSource> oldItems)
        {
            OnCollectionChanged(ObservableCollectionChangedEventArgs<TResult>.ReplacedRange(newItems.SelectList(MapItem), oldItems.SelectList(MapItem), previousIndex));

            foreach (TSource src in oldItems)
            {
                Decrement(src);
            }

            foreach (TSource src in newItems)
            {
                Increment(src);
            }
        }

        private TResult MapItem(TSource item)
        {
            if (_mapping == null)
                return _selector(item);

            if (!_mapping.TryGetValue(item, out var result))
            {
                result = (_selector(item), 1);

                _mapping.Add(item, result);
            }

            return result.Result;
        }

        private void Increment(TSource item)
        {
            if (!_mapping.TryGetValue(item, out var result))
            {
                result = (_selector(item), 0);
            }

            result.ReferenceCount += 1;

            _mapping[item] = result;
        }

        private void Decrement(TSource item)
        {
            if (_mapping.TryGetValue(item, out var result))
            {
                int c = result.ReferenceCount - 1;

                if (c == 0)
                    _mapping.Remove(item);
                else
                    _mapping[item] = (result.Result, c);
            }
        }

        public override IEnumerable<TResult> GetItems()
        {
            return UnderlyingInstance.GetItems().Select(MapItem);
        }
    }
}
