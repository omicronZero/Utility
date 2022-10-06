using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public class ExternalProperty<T> : ObservableProperty<T>
    {
        private T _value;
        
        private readonly DisposableList<IObserver> _observers;
        private ReadonlyObservableProperty<T> _readonlyProperty;

        public ReadonlyObservableProperty<T> AsReadonly()
        {
            return _readonlyProperty;
        }

        public ExternalProperty(T initialValue)
        {
            _value = initialValue;
            _observers = new DisposableList<IObserver>();
            _readonlyProperty = new ReadonlyObservableProperty<T>(this);
        }

        public override T Value
        {
            get => _value;
            set => _value = value;
        }

        public override bool IsReadonly => false;

        public override IDisposable RegisterObserver(IObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            return _observers.Add(observer);
        }
    }
}
