using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Utility.Collections.ObjectModel
{
    public delegate bool ConverterPredicate<TIn, TOut>(TIn input, out TOut output);

    public class ObservableFilter<TIn, TOut> : IEnumerable<TOut>, INotifyCollectionChanged
    {
        private NotifyCollectionChangedEventHandler _collectionChangedEvent;

        protected IEnumerable<TIn> UnderlyingEnumerable { get; }
        private readonly ConverterPredicate<TIn, TOut> _predicate;

        public ObservableFilter(IEnumerable<TIn> underlyingEnumerable, ConverterPredicate<TIn, TOut> predicate)
        {

            UnderlyingEnumerable = underlyingEnumerable ?? throw new ArgumentNullException(nameof(underlyingEnumerable));
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            if (!(underlyingEnumerable is INotifyCollectionChanged))
                throw new ArgumentException($"Notifying collection of type {typeof(INotifyCollectionChanged).FullName} expected.", nameof(underlyingEnumerable));
        }

        public ObservableFilter(IEnumerable<TIn> underlyingList, Func<TIn, bool> predicate, Func<TIn, TOut> selector)
            : this(underlyingList, CombinePredicateSelector(predicate, selector))
        { }

        private INotifyCollectionChanged NotifyCollection => (INotifyCollectionChanged)UnderlyingEnumerable;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                var previousHandler = _collectionChangedEvent;

                _collectionChangedEvent += value;

                if (previousHandler == null)
                {
                    NotifyCollection.CollectionChanged += NotifyCollection_CollectionChanged;
                }
            }
            remove
            {
                _collectionChangedEvent -= value;

                if (_collectionChangedEvent == null)
                {
                    NotifyCollection.CollectionChanged -= NotifyCollection_CollectionChanged;
                }
            }
        }

        private IEnumerable<TOut> Filter(IEnumerable<TIn> values)
        {
            foreach (var v in values)
            {
                if (_predicate(v, out var output))
                    yield return output;
            }
        }

        protected virtual void ProcessInnerCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        Array.AsReadOnly(Filter(e.NewItems.Cast<TIn>()).ToArray()))
                    );
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        Array.AsReadOnly(Filter(e.OldItems.Cast<TIn>()).ToArray()))
                    );
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        Array.AsReadOnly(Filter(e.NewItems.Cast<TIn>()).ToArray()),
                        Array.AsReadOnly(Filter(e.OldItems.Cast<TIn>()).ToArray()),
                        -1)
                    );
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private void NotifyCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
                return;

            ProcessInnerCollectionChanged(e);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            _collectionChangedEvent?.Invoke(this, e);
        }

        public IEnumerator<TOut> GetEnumerator()
        {
            return Filter(UnderlyingEnumerable).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static ConverterPredicate<TIn, TOut> CombinePredicateSelector(Func<TIn, bool> predicate, Func<TIn, TOut> selector)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            bool Convert(TIn input, out TOut output)
            {
                if (!predicate(input))
                {
                    output = default;
                    return false;
                }

                output = selector(input);
                return true;
            }

            return Convert;
        }
    }

    public class ObservableFilter<T> : ObservableFilter<T, T>
    {
        private static readonly Func<T, T> passDelegate = Pass;

        public ObservableFilter(IEnumerable<T> underlyingList, Func<T, bool> predicate)
            : base(underlyingList, predicate, passDelegate)
        { }

        private static T Pass(T value) => value;
    }
}
