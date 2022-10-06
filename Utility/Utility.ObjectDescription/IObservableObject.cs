using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public interface IObservableObject
    {
        IDisposable RegisterObserver(IObserver observer);
        IReadOnlyList<IObjectProperty> GetObjectProperties();
    }
}
