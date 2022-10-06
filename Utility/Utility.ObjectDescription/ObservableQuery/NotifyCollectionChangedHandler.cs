using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Utility.Collections;

namespace Utility.ObjectDescription.ObservableQuery
{
    public abstract class NotifyCollectionChangedHandler<T>
    {
        protected INotifyCollectionChanged UnderlyingInstance { get; }

        protected abstract void ResetItems();
        protected abstract void InsertItems(int previousIndex, IList<T> items);
        protected abstract void RemoveItems(int previousIndex, IList<T> items);
        protected abstract void SetItems(int previousIndex, IList<T> newItems, IList<T> oldItems);
        protected abstract void MoveItems(int newPreviousIndex, IList<T> items, int oldPreviousIndex);

        protected void BeginListen()
        {
            UnderlyingInstance.CollectionChanged += _previous_CollectionChanged;
        }

        protected void EndListen()
        {
            UnderlyingInstance.CollectionChanged -= _previous_CollectionChanged;
        }

        protected virtual void OnPreviousChanged()
        { }

        private void _previous_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                InsertItems(e.NewStartingIndex, e.NewItems?.CastList<T>());
            else if (e.Action == NotifyCollectionChangedAction.Move)
                MoveItems(e.NewStartingIndex, e.NewItems?.CastList<T>(), e.OldStartingIndex);
            else if (e.Action == NotifyCollectionChangedAction.Remove)
                RemoveItems(e.NewStartingIndex, e.OldItems?.CastList<T>());
            else if (e.Action == NotifyCollectionChangedAction.Replace)
                SetItems(e.NewStartingIndex, e.NewItems?.CastList<T>(), e.OldItems?.CastList<T>());
            else if (e.Action == NotifyCollectionChangedAction.Reset)
                ResetItems();

            OnPreviousChanged();
        }

        public NotifyCollectionChangedHandler(INotifyCollectionChanged underlyingInstance)
        {
            if (underlyingInstance == null)
                throw new ArgumentNullException(nameof(underlyingInstance));

            UnderlyingInstance = underlyingInstance;
        }
    }
}
