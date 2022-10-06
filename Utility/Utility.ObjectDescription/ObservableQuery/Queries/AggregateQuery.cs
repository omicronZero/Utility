using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription.ObservableQuery.Queries
{
    public delegate TAccumulate AggregateQueryHandler<TAccumulate, TSource>(TAccumulate accumulate, TSource value, out bool stop);

    public class AggregateQuery<TSource, TAccumulate, TResult> : NotifyCollectionChangedHandler<TSource>, IObservableProperty<TResult>, IDisposable
    {
        private readonly DisposableList<IObserver<TResult>> _observers;

        private readonly Func<TAccumulate> _seedProvider;
        private readonly AggregateQueryHandler<TAccumulate, TSource> _func;
        private readonly Func<TAccumulate, TResult> _resultSelector;

        private TResult _value;
        private TAccumulate _valueAccumulate;

        private int _previousCount;

        protected bool IsDisposed { get; private set; }

        new protected IObservationListQuery<TSource> UnderlyingInstance => (IObservationListQuery<TSource>)base.UnderlyingInstance;

        public AggregateQuery(IObservationListQuery<TSource> underlyingQuery, Func<TAccumulate> seedProvider, AggregateQueryHandler<TAccumulate, TSource> func, Func<TAccumulate, TResult> resultSelector)
            : base(underlyingQuery)
        {
            if (seedProvider == null)
                throw new ArgumentNullException(nameof(seedProvider));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            _observers = new DisposableList<IObserver<TResult>>();

            _seedProvider = seedProvider;
            _func = func;
            _resultSelector = resultSelector;

            BeginListen();
        }

        public TResult Value
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(this.GetType().Name);

                return _value;
            }
            set => throw new NotSupportedException("The current property is read-only.");
        }

        public bool IsReadonly => true;

        public IDisposable RegisterObserver(IObserver<TResult> observer)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);

            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            return _observers.Add(observer);
        }

        private void PropertyChanged(TResult oldValue)
        {
            foreach (IObserver<TResult> observer in _observers)
                observer.OnPropertyChanged(this, oldValue);
        }

        protected override void InsertItems(int previousIndex, IList<TSource> items)
        {
            if (items.Count != 0)
            {
                if (previousIndex == _previousCount)
                {
                    Compute(items, _valueAccumulate);
                }
                else
                    ResetItems();
            }
        }

        protected override void MoveItems(int newPreviousIndex, IList<TSource> items, int oldPreviousIndex)
        {
            if (items.Count != 0 && newPreviousIndex != oldPreviousIndex)
                ResetItems();
        }

        protected override void RemoveItems(int previousIndex, IList<TSource> items)
        {
            if (items.Count != 0)
                ResetItems();
        }

        protected override void ResetItems()
        {
            _previousCount = 0;
            Compute(UnderlyingInstance.GetItems(), _seedProvider());
        }

        private void Compute(IEnumerable<TSource> items, TAccumulate accumulate)
        {
            TResult oldResult = _value;
            bool b;

            int c = _previousCount;

            foreach (TSource s in items)
            {
                accumulate = _func(accumulate, s, out b);
                c++;

                if (b)
                    break;
            }

            _valueAccumulate = accumulate;
            _value = _resultSelector(accumulate);
            _previousCount = c;

            PropertyChanged(oldResult);
        }

        protected override void SetItems(int previousIndex, IList<TSource> newItems, IList<TSource> oldItems)
        {
            if (newItems.Count != 0 && oldItems.Count != 0)
                ResetItems();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                IsDisposed = true;

                _observers.Clear();
                _valueAccumulate = default;
                _value = default;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
