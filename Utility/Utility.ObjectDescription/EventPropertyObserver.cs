using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public class EventPropertyObserver<T> : Observer<T>
    {
        public event EventHandler<ObservablePropertyChangedEventArgs<T>> Changed;
        public event EventHandler<ObservablePropertyEventArgs<T>> Registered;
        public event EventHandler<ObservablePropertyEventArgs<T>> Unregistered;

        public override void OnPropertyChanged(IObservableProperty<T> property, T oldValue)
        {
            Changed?.Invoke(this, new ObservablePropertyChangedEventArgs<T>(property, oldValue));
        }

        public override void OnRegistered(IObservableProperty<T> property)
        {
            base.OnRegistered(property);
            Registered?.Invoke(this, new ObservablePropertyEventArgs<T>(property));
        }

        public override void OnUnregistered(IObservableProperty<T> property)
        {
            base.OnUnregistered(property);
            Unregistered?.Invoke(this, new ObservablePropertyEventArgs<T>(property));
        }
    }

    public class EventPropertyObserver : Observer
    {
        public event EventHandler<ObservablePropertyChangedEventArgs> Changed;
        public event EventHandler<ObservablePropertyEventArgs> Registered;
        public event EventHandler<ObservablePropertyEventArgs> Unregistered;

        public override void OnPropertyChanged(IObservableProperty property, object oldValue)
        {
            Changed?.Invoke(this, new ObservablePropertyChangedEventArgs(property, oldValue));
        }

        public override void OnRegistered(IObservableProperty property)
        {
            base.OnRegistered(property);
            Registered?.Invoke(this, new ObservablePropertyEventArgs(property));
        }

        public override void OnUnregistered(IObservableProperty property)
        {
            base.OnUnregistered(property);
            Unregistered?.Invoke(this, new ObservablePropertyEventArgs(property));
        }
    }
}
