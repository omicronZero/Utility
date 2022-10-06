using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public abstract class GenericObservableProperty<T> : ObservableProperty<T>
    {
        private readonly DisposableList<IObserver> _observers;

        protected GenericObservableProperty(bool synchronizedObserverList)
        {
            _observers = new DisposableList<IObserver>(synchronizedObserverList);
        }

        public override IDisposable RegisterObserver(IObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            IDisposable h = _observers.Add(observer, (o) => o.OnUnregistered(this));

            try
            {
                observer.OnRegistered(this);
            }
            catch
            {
                h.Dispose();
                throw;
            }

            return h;
        }
    }
}
