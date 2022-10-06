using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public interface IObservableProperty<T> : IObjectProperty<T>
    {
        IDisposable RegisterObserver(IObserver<T> observer);
    }

    public interface IObservableProperty : IObjectProperty
    {
        IDisposable RegisterObserver(IObserver observer);
    }
}
