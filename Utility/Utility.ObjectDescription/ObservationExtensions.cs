using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Utility.ObjectDescription.ObservableQuery;

namespace Utility.ObjectDescription
{
    public static class ObservationExtensions
    {
        public static IObservationListQuery<T> AsQueryable<T>(IObservableList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return new ListWrapperQuery<T>(list);
        }

        private sealed class ListWrapperQuery<T> : IObservationListQuery<T>
        {
            private readonly IObservableList<T> _list;

            private NotifyCollectionChangedEventHandler _handler;

            public event NotifyCollectionChangedEventHandler CollectionChanged
            {
                add
                {
                    NotifyCollectionChangedEventHandler h = _handler;

                    _handler += value;

                    if (h == null && _handler != null)
                        _list.CollectionChanged += _list_CollectionChanged;
                }
                remove
                {
                    NotifyCollectionChangedEventHandler h = _handler;

                    _handler -= value;

                    if (h != null && _handler == null)
                        _list.CollectionChanged -= _list_CollectionChanged;
                }
            }

            private void _list_CollectionChanged(object sender, ObservableCollectionChangedEventArgs<T> e)
            {
                _handler?.Invoke(this, e);
            }

            public ListWrapperQuery(IObservableList<T> list)
            {
                if (list == null)
                    throw new ArgumentNullException(nameof(list));

                _list = list;
            }

            public IEnumerable<T> GetItems()
            {
                return _list;
            }

            public void Reset()
            { }
        }
    }
}
