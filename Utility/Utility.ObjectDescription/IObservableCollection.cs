using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public interface IObservableCollection<T> : ICollection<T>, IReadOnlyCollection<T>, System.Collections.Specialized.INotifyCollectionChanged
    {
        new event EventHandler<ObservableCollectionChangedEventArgs<T>> CollectionChanged;
    }
}
