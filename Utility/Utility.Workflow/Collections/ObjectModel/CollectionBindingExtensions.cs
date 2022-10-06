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
        public static IDisposable BindUnorderedTo<TTarget, TSource>(this ICollection<TTarget> targetCollection, ICollection<TSource> sourceCollection, Func<TSource, TTarget> selector)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));
            if (targetCollection == null)
                throw new ArgumentNullException(nameof(targetCollection));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new NotifyCollectionChangedBinder<TTarget, TSource>(targetCollection, sourceCollection, selector, true);
        }

        public static IDisposable BindTo<TTarget, TSource>(this IList<TTarget> targetCollection, IList<TSource> sourceCollection, Func<TSource, TTarget> selector)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));
            if (targetCollection == null)
                throw new ArgumentNullException(nameof(targetCollection));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new NotifyCollectionChangedBinder<TTarget, TSource>(targetCollection, sourceCollection, selector, false);
        }
        public static IDisposable BindUnorderedToOrReset<TTarget, TSource>(this ICollection<TTarget> targetCollection, ICollection<TSource> sourceCollection, Func<TSource, TTarget> selector)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));
            if (targetCollection == null)
                throw new ArgumentNullException(nameof(targetCollection));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (targetCollection is INotifyCollectionChanged)
                return new NotifyCollectionChangedBinder<TTarget, TSource>(targetCollection, sourceCollection, selector, true);
            else
            {
                targetCollection.Clear();
                foreach (var s in sourceCollection)
                    targetCollection.Add(selector(s));

                return Disposable.Create(() => { });
            }
        }

        public static IDisposable BindToOrReset<TTarget, TSource>(this IList<TTarget> targetCollection, IList<TSource> sourceCollection, Func<TSource, TTarget> selector)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));
            if (targetCollection == null)
                throw new ArgumentNullException(nameof(targetCollection));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (targetCollection is INotifyCollectionChanged)
                return new NotifyCollectionChangedBinder<TTarget, TSource>(targetCollection, sourceCollection, selector, false);
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
            private readonly ICollection<TSource> _sourceCollection;
            private readonly Func<TSource, TTarget> _selector;
            private readonly bool _unordered;

            public NotifyCollectionChangedBinder(
                ICollection<TTarget> targetCollection,
                ICollection<TSource> sourceCollection,
                Func<TSource, TTarget> selector,
                bool unordered)
            {
                _targetCollection = targetCollection ?? throw new ArgumentNullException(nameof(targetCollection));
                _sourceCollection = sourceCollection ?? throw new ArgumentNullException(nameof(sourceCollection));
                _selector = selector ?? throw new ArgumentNullException(nameof(selector));
                _unordered = unordered || !(_targetCollection is IList<TTarget>);

                if (!(sourceCollection is INotifyCollectionChanged ntfy))
                    throw new ArgumentException("The source collection must be observable.", nameof(sourceCollection));

                Reset();
                ntfy.CollectionChanged += Ntfy_CollectionChanged;
            }

            public void Reset()
            {
                _targetCollection.Clear();

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
                            targetList!.RemoveAt(i);
                    }
                    else
                    {
                        int i = e.NewStartingIndex;

                        foreach (var _ in oldItems)
                            targetList!.RemoveAt(i);
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
                        targetList!.Insert(i, items[i]);
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
                            _targetCollection.Remove(_selector(item));
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
                            targetList![i++] = _selector(item);
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
