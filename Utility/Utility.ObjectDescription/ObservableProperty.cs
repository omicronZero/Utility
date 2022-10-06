using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public abstract class ObservableProperty<T> : ObjectProperty<T>, IObservableProperty<T>, IObservableProperty
    {
        public abstract IDisposable RegisterObserver(IObserver observer);

        IDisposable IObservableProperty<T>.RegisterObserver(IObserver<T> observer)
        {
            return RegisterObserver(observer);
        }
    }
}
