using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Collections.ObjectModel
{
    public static class CollectionBindingExtensions
    {
        public static IDisposable BindUnorderedTo<TTarget, TSource>(this ICollection<TTarget> targetCollection, IEnumerable<TSource> sourceCollection, Func<TSource, TTarget> selector, Action<TTarget> disposer = null)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));
            if (targetCollection == null)
                throw new ArgumentNullException(nameof(targetCollection));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new NotifyCollectionChangedBinder<TTarget, TSource>(targetCollection, sourceCollection, selector, disposer, true);
        }

        public static IDisposable BindTo<TTarget, TSource>(this IList<TTarget> targetCollection, IList<TSource> sourceCollection, Func<TSource, TTarget> selector, Action<TTarget> disposer = null)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));
            if (targetCollection == null)
                throw new ArgumentNullException(nameof(targetCollection));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new NotifyCollectionChangedBinder<TTarget, TSource>(targetCollection, sourceCollection, selector, disposer, false);
        }
        public static IDisposable BindUnorderedToOrReset<TTarget, TSource>(this ICollection<TTarget> targetCollection, IEnumerable<TSource> sourceCollection, Func<TSource, TTarget> selector, Action<TTarget> disposer = null)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));
            if (targetCollection == null)
                throw new ArgumentNullException(nameof(targetCollection));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (targetCollection is INotifyCollectionChanged)
                return new NotifyCollectionChangedBinder<TTarget, TSource>(targetCollection, sourceCollection, selector, disposer, true);
            else
            {
                targetCollection.Clear();
                foreach (var s in sourceCollection)
                    targetCollection.Add(selector(s));

                return Disposable.Create(() => { });
            }
        }

        public static IDisposable BindToOrReset<TTarget, TSource>(this IList<TTarget> targetCollection, IList<TSource> sourceCollection, Func<TSource, TTarget> selector, Action<TTarget> disposer = null)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));
            if (targetCollection == null)
                throw new ArgumentNullException(nameof(targetCollection));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (targetCollection is INotifyCollectionChanged)
                return new NotifyCollectionChangedBinder<TTarget, TSource>(targetCollection, sourceCollection, selector, disposer, false);
            else
            {
                targetCollection.Clear();
                foreach (var s in sourceCollection)
                    targetCollection.Add(selector(s));

                return Disposable.Create(() => { });
            }
        }

        private sealed class NotifyCollectionChangedBinder<TTarget, TSource> : IDisposable
        {
            private bool _disposed;

            private readonly ICollection<TTarget> _targetCollection;
            private readonly IEnumerable<TSource> _sourceCollection;
            private readonly Func<TSource, TTarget> _selector;
            private readonly Action<TTarget> _disposer;
            private readonly bool _unordered;

            public NotifyCollectionChangedBinder(
                ICollection<TTarget> targetCollection,
                IEnumerable<TSource> sourceCollection,
                Func<TSource, TTarget> selector,
                Action<TTarget> disposer,
                bool unordered)
            {
                _targetCollection = targetCollection ?? throw new ArgumentNullException(nameof(targetCollection));
                _sourceCollection = sourceCollection ?? throw new ArgumentNullException(nameof(sourceCollection));
                _selector = selector ?? throw new ArgumentNullException(nameof(selector));
                _unordered = unordered || !(_targetCollection is IList<TTarget>);
                _disposer = disposer;

                if (!(sourceCollection is INotifyCollectionChanged ntfy))
                    throw new ArgumentException("The source collection must be observable.", nameof(sourceCollection));

                Reset();
                ntfy.CollectionChanged += Ntfy_CollectionChanged;
            }

            public void Reset()
            {
                if (_disposer == null)
                {
                    _targetCollection.Clear();
                }
                else
                {
                    var items = _targetCollection.ToArray();
                    _targetCollection.Clear();

                    foreach (var item in items)
                        _disposer(item);
                }

                foreach (var item in _sourceCollection)
                    _targetCollection.Add(_selector(item));
            }

            private void Ntfy_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                    Reset();
                else if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var newItems = (e.NewItems ?? throw new ArgumentException("NewItems was not expected to be null.", nameof(e))).Cast<TSource>();
                    var targetList = _targetCollection as IList<TTarget>;

                    if (_unordered || e.NewStartingIndex == -1)
                    {
                        foreach (var item in newItems)
                            _targetCollection.Add(_selector(item));
                    }
                    else
                    {
                        int i = e.NewStartingIndex;

                        foreach (var item in newItems)
                            targetList!.Insert(i++, _selector(item));
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    var oldItems = (e.OldItems ?? throw new ArgumentException("OldItems was not expected to be null.", nameof(e))).Cast<TSource>();
                    var targetList = _targetCollection as IList<TTarget>;

                    //TODO: specially deal with RemoveRange of IList<T>

                    if (_unordered || e.NewStartingIndex == -1)
                    {
                        foreach (var item in oldItems)
                            _targetCollection.Add(_selector(item));
                    }
                    else if (oldItems is ICollection<TTarget> coll)
                    {
                        int c = coll.Count;

                        for (int i = e.NewStartingIndex + c - 1; i >= 0; i--)
                        {
                            var oldItem = targetList![i];
                            targetList!.RemoveAt(i);
                            _disposer?.Invoke(oldItem);
                        }
                    }
                    else
                    {
                        int i = e.NewStartingIndex;

                        var disposer = _disposer;

                        if (disposer == null)
                        {
                            foreach (var _ in oldItems)
                            {
                                targetList!.RemoveAt(i);
                            }
                        }
                        else
                        {
                            foreach (var item in oldItems)
                            {
                                var oldItem = _selector(item);
                                targetList!.RemoveAt(i);
                                disposer?.Invoke(oldItem);
                            }
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Move)
                {
                    if (_unordered)
                        return;

                    int oldIndex = e.OldStartingIndex;
                    int newIndex = e.NewStartingIndex;

                    if (oldIndex == -1 && newIndex == -1)
                        throw new InvalidOperationException("OldStartingIndex and NewStartingIndex must be set to a non-negative value for Move-operations.");

                    var newItems = (e.NewItems ?? throw new ArgumentException("NewItems was not expected to be null.", nameof(e))).Cast<TSource>();

                    int count = newItems.Count();
                    TTarget[] items = new TTarget[count];
                    var targetList = _targetCollection as IList<TTarget>;

                    for (int i = count - 1; i >= 0; i--)
                    {
                        items[i] = targetList![oldIndex + i];
                        targetList.RemoveAt(oldIndex + i);
                    }

                    for (int i = 0; i < count; i++)
                    {
                        targetList!.Insert(newIndex + i, items[i]);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    var newItems = (e.NewItems ?? throw new ArgumentException("NewItems was not expected to be null.", nameof(e))).Cast<TSource>();
                    var targetList = _targetCollection as IList<TTarget>;

                    if (_unordered || e.NewStartingIndex == -1)
                    {
                        var oldItems = (e.OldItems ?? throw new ArgumentException("OldItems was not expected to be null.", nameof(e))).Cast<TSource>();

                        foreach (var item in oldItems)
                        {
                            var oldItem = _selector(item);
                            _targetCollection.Remove(oldItem);
                            _disposer?.Invoke(oldItem);
                        }

                        foreach (var item in newItems)
                        {
                            _targetCollection.Add(_selector(item));
                        }
                    }
                    else
                    {
                        int i = e.NewStartingIndex;

                        foreach (var item in newItems)
                        {
                            var oldItem = targetList![i];
                            targetList![i++] = _selector(item);
                            _disposer?.Invoke(oldItem);
                        }
                    }
                }
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;

                    ((INotifyCollectionChanged)_sourceCollection).CollectionChanged -= Ntfy_CollectionChanged;
                }
            }
        }
    }
}
