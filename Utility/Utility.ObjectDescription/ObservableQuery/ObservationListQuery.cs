using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Utility.ObjectDescription.ObservableQuery
{
    public abstract class ObservationListQuery<TSource, TResult> : NotifyCollectionChangedHandler<TSource>, IObservationListQuery<TResult>
    {
        private NotifyCollectionChangedEventHandler _handler;

        public abstract IEnumerable<TResult> GetItems();

        new protected IObservationListQuery<TSource> UnderlyingInstance => (IObservationListQuery<TSource>)base.UnderlyingInstance;

        public ObservationListQuery(IObservationListQuery<TSource> underlyingQuery) : base(underlyingQuery)
        { }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                NotifyCollectionChangedEventHandler h = _handler;

                _handler += value;

                if (h == null && _handler != null)
                    BeginListen();
            }
            remove
            {
                NotifyCollectionChangedEventHandler h = _handler;
                
                _handler -= value;

                if (h != null && _handler == null)
                    EndListen();
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            _handler?.Invoke(this, e);
        }

        public virtual void Reset()
        {
            ResetItems();
        }
    }
}
