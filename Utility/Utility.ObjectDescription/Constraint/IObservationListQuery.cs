using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Utility.ObjectDescription.ObservableQuery
{
    public interface IObservationListQuery<T> : INotifyCollectionChanged, IQueryResult
    {
        IEnumerable<T> GetItems();
    }
}
