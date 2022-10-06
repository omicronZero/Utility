using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public abstract class Observer<T> : IObserver<T>
    {
        public abstract void OnPropertyChanged(IObservableProperty<T> property, T oldValue);

        public virtual void OnRegistered(IObservableProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
        }

        public virtual void OnRegistered(IObservableObject instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
        }

        public virtual void OnUnregistered(IObservableProperty<T> property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
        }

        public virtual void OnUnregistered(IObservableObject instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
        }

        void IObserver.OnPropertyChanged(IObservableProperty property, object oldValue)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (property is IObservableProperty<T> p)
            {
                T v;

                if (oldValue is T cv)
                {
                    v = cv;
                }
                else
                {
                    throw new ArgumentException($"Type mismatch. Old value is expected to be of type { typeof(T).FullName }.", nameof(oldValue));
                }

                OnPropertyChanged(p, v);
            }
        }

        void IObserver.OnRegistered(IObservableProperty property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (property is IObservableProperty<T> p)
                OnRegistered(p);
        }

        void IObserver.OnUnregistered(IObservableProperty property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (property is IObservableProperty<T> p)
                OnUnregistered(p);
        }

        public static IObserver<T> FromDelegate(Action<IObservableProperty<T>, T> observationHandler)
        {
            if (observationHandler == null)
                throw new ArgumentNullException(nameof(observationHandler));

            return new DelegateObserver(observationHandler);
        }

        private sealed class DelegateObserver : Observer<T>
        {
            private Action<IObservableProperty<T>, T> _observationHandler;

            public DelegateObserver(Action<IObservableProperty<T>, T> observationHandler)
            {
                if (observationHandler == null)
                    throw new ArgumentNullException(nameof(observationHandler));

                _observationHandler = observationHandler;
            }

            public override void OnPropertyChanged(IObservableProperty<T> property, T oldValue)
            {
                _observationHandler(property, oldValue);
            }
        }
    }

    public abstract class Observer : IObserver
    {
        public abstract void OnPropertyChanged(IObservableProperty property, object oldValue);

        public virtual void OnRegistered(IObservableObject instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
        }

        public virtual void OnRegistered(IObservableProperty property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
        }

        public virtual void OnUnregistered(IObservableObject instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
        }

        public virtual void OnUnregistered(IObservableProperty property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
        }

        public static IObserver FromDelegate(Action<IObservableProperty, object> observationHandler)
        {
            if (observationHandler == null)
                throw new ArgumentNullException(nameof(observationHandler));

            return new DelegateObserver(observationHandler);
        }

        private sealed class DelegateObserver : Observer
        {
            private Action<IObservableProperty, object> _observationHandler;

            public DelegateObserver(Action<IObservableProperty, object> observationHandler)
            {
                if (observationHandler == null)
                    throw new ArgumentNullException(nameof(observationHandler));

                _observationHandler = observationHandler;
            }

            public override void OnPropertyChanged(IObservableProperty property, object oldValue)
            {
                _observationHandler(property, oldValue);
            }
        }
    }
}
