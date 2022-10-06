using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public class ReadonlyObservableProperty<T> : IObservableProperty<T>
    {
        private readonly IObservableProperty<T> _property;

        public T Value
        {
            get => _property.Value;
            set => throw new NotSupportedException("The property is read-only.");
        }

        public bool IsReadonly => true;

        public IDisposable RegisterObserver(IObserver<T> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            return _property.RegisterObserver(Observer<T>.FromDelegate((p, ov) => observer.OnPropertyChanged(this, ov)));
        }

        public ReadonlyObservableProperty(IObservableProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            _property = property;
        }
    }
}
