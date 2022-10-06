using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Utility.Collections;
using Utility;

namespace Utility.ObjectDescription
{
    public class ObservableCollectionChangedEventArgs<T> : NotifyCollectionChangedEventArgs
    {
        new public IList<T> OldItems => (IList<T>)base.OldItems;
        new public IList<T> NewItems => (IList<T>)base.NewItems;

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action)
            : base(action)
        { }

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> changedItems)
            : base(action, changedItems)
        { }

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, T changedItem)
            : this(action, ListExtensions.Single(changedItem))
        { }

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> newItems, IList<T> oldItems)
            : base(action, newItems, oldItems)
        { }

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> changedItems, int startingIndex)
            : base(action, changedItems, startingIndex)
        { }

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, T changedItem, int index)
            : this(action, ListExtensions.Single(changedItem), index)
        { }

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, T newItem, T oldItem)
            : this(action, ListExtensions.Single(newItem), ListExtensions.Single(oldItem))
        { }

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> newItems, IList<T> oldItems, int startingIndex)
            : base(action, newItems, oldItems, startingIndex)
        { }

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> changedItems, int index, int oldIndex)
            : base(action, changedItems, index, oldIndex)
        { }

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, T changedItem, int index, int oldIndex)
            : this(action, ListExtensions.Single(changedItem), index, oldIndex)
        { }

        public ObservableCollectionChangedEventArgs(NotifyCollectionChangedAction action, T newItem, T oldItem, int index)
            : this(action, ListExtensions.Single(newItem), ListExtensions.Single(oldItem), index)
        { }

        public static ObservableCollectionChangedEventArgs<T> Reset()
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Reset);
        }

        public static ObservableCollectionChangedEventArgs<T> AddedRange(IList<T> changedItems)
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Add, changedItems);
        }

        public static ObservableCollectionChangedEventArgs<T> InsertedRange(IList<T> changedItems, int startIndex)
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Add, changedItems, startIndex);
        }

        public static ObservableCollectionChangedEventArgs<T> Added(T changedItem)
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Add, changedItem);
        }

        public static ObservableCollectionChangedEventArgs<T> Inserted(T changedItem, int index)
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Add, changedItem, index);
        }

        public static ObservableCollectionChangedEventArgs<T> Removed(T removedItem, int index)
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        public static ObservableCollectionChangedEventArgs<T> RemovedRange(IList<T> removedItems, int startIndex)
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Remove, removedItems, startIndex);
        }

        public static ObservableCollectionChangedEventArgs<T> Replaced(T newItem, T oldItem, int index)
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Replace, newItem, oldItem, index);
        }

        public static ObservableCollectionChangedEventArgs<T> ReplacedRange(IList<T> newItems, IList<T> oldItems, int startIndex)
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Replace, newItems, oldItems, startIndex);
        }

        public static ObservableCollectionChangedEventArgs<T> Moved(T item, int index, int oldIndex)
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Move, item, index, oldIndex);
        }

        public static ObservableCollectionChangedEventArgs<T> MovedRange(IList<T> items, int newStartIndex, int oldStartIndex)
        {
            return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Move, items, newStartIndex, oldStartIndex);
        }

        public static ObservableCollectionChangedEventArgs<T> From(NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
                return null;

            if (args is ObservableCollectionChangedEventArgs<T> coll)
                return coll;

            if (!(args.NewItems is IList<T> nl))
                throw new ArgumentException($"List of new items expected to hold items of type {typeof(T).FullName}.");

            if (!(args.OldItems is IList<T> ol))
                throw new ArgumentException($"List of old items expected to hold items of type {typeof(T).FullName}.");

            if (args.Action == NotifyCollectionChangedAction.Move)
            {
                return new ObservableCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Move, nl, args.NewStartingIndex, args.OldStartingIndex);
            }
            else
                return new ObservableCollectionChangedEventArgs<T>(args.Action, nl, ol, args.NewStartingIndex);
        }
    }
}