using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public interface IObservableList<T> : IList<T>, IReadOnlyList<T>, System.Collections.Specialized.INotifyCollectionChanged
    {
        new event EventHandler<ObservableCollectionChangedEventArgs<T>> CollectionChanged;
    }
}
