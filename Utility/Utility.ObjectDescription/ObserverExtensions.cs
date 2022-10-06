using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.ObjectDescription
{
    public static class ObserverExtensions
    {
        public static IObserver<T> Create<T>(Action<IObservableProperty<T>, T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return new DelegateObserver<T>(handler);
        }

        public static IDisposable RegisterObserverWeak(this IObservableObject observable, IObserver observer)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable));
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            return new WeakObserver(observer, observable).Handle;
        }

        public static IDisposable RegisterObserverWeak<T>(this IObservableProperty<T> observableProperty, IObserver<T> observer)
        {
            if (observableProperty == null)
                throw new ArgumentNullException(nameof(observableProperty));
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            return new WeakObserver<T>(observer, observableProperty).Handle;
        }

        public static IDisposable RegisterObserverWeak(this IObservableProperty observableProperty, IObserver observer)
        {
            if (observableProperty == null)
                throw new ArgumentNullException(nameof(observableProperty));
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            return new WeakObserver(observer, observableProperty).Handle;
        }

        public static IDisposable Bind<TProperty, TBinding>(
            this IObservableProperty<TProperty> instance,
            IObservableProperty<TBinding> property,
            Func<TBinding, TProperty> valueSelector)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            if (valueSelector == null)
                throw new ArgumentNullException(nameof(valueSelector));
            if (instance.IsReadonly)
                throw new ArgumentException("The specified destination property is read-only.", nameof(instance));

            return property.RegisterObserver(Create<TBinding>((p, o) => instance.Value = valueSelector(p.Value)));
        }

        public static IObjectProperty<T> Cast<T>(IObjectProperty property)
        {
            if (property == null)
                return null;

            if (property is IObjectProperty<T> p)
                return p;

            return new ObjectPropertyCast<T>(property);
        }

        public static IObservableProperty<T> Cast<T>(IObservableProperty property)
        {
            if (property == null)
                return null;

            if (property is IObservableProperty<T> p)
                return p;

            return new ObservablePropertyCast<T>(property);
        }

        public static IDisposable Bind<TProperty>(
            this IObservableProperty<TProperty> instance,
            IObservableProperty<TProperty> property)
        {
            return Bind(instance, property, (v) => v);
        }

        private sealed class DelegateObserver<T> : Observer<T>
        {
            private readonly Action<IObservableProperty<T>, T> _handler;

            public DelegateObserver(Action<IObservableProperty<T>, T> handler)
            {
                if (handler == null)
                    throw new ArgumentNullException(nameof(handler));

                _handler = handler;
            }

            public override void OnPropertyChanged(IObservableProperty<T> property, T oldValue)
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));

                _handler(property, oldValue);
            }
        }

        private sealed class ObjectPropertyCast<T> : IObjectProperty<T>
        {
            private readonly IObjectProperty _property;

            public ObjectPropertyCast(IObjectProperty property)
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));

                _property = property;
            }

            public bool IsReadonly => _property.IsReadonly;

            public T Value
            {
                get => (T)_property.Value;
                set => _property.Value = value;
            }
        }

        private sealed class ObservablePropertyCast<T> : IObservableProperty<T>
        {
            private readonly IObservableProperty _property;

            public ObservablePropertyCast(IObservableProperty property)
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));

                _property = property;
            }

            public bool IsReadonly => _property.IsReadonly;

            public T Value
            {
                get => (T)_property.Value;
                set => _property.Value = value;
            }

            public IDisposable RegisterObserver(IObserver<T> observer)
            {
                if (observer == null)
                    throw new ArgumentNullException(nameof(observer));

                return _property.RegisterObserver(observer);
            }
        }

        private sealed class WeakObserver : IObserver
        {
            private WeakReference<IObserver> _innerObserver;
            public IDisposable Handle { get; private set; }

            public WeakObserver(IObserver innerObserver, IObservableObject observable)
            {
                if (innerObserver == null)
                    throw new ArgumentNullException(nameof(innerObserver));
                if (observable == null)
                    throw new ArgumentNullException(nameof(observable));

                _innerObserver = new WeakReference<IObserver>(innerObserver);

                Handle = observable.RegisterObserver(this);

                CreateDisposable();
            }

            public WeakObserver(IObserver innerObserver, IObservableProperty observableProperty)
            {
                if (innerObserver == null)
                    throw new ArgumentNullException(nameof(innerObserver));
                if (observableProperty == null)
                    throw new ArgumentNullException(nameof(observableProperty));

                _innerObserver = new WeakReference<IObserver>(innerObserver);

                Handle = observableProperty.RegisterObserver(this);

                CreateDisposable();
            }

            private void CreateDisposable()
            {
                IDisposable p = Handle;

                Handle = Disposable.Create(() =>
                {
                    p?.Dispose();
                    p = null;
                    _innerObserver = null;
                });
            }

            public void OnPropertyChanged(IObservableProperty property, object oldValue)
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));

                IObserver inner;

                if (_innerObserver.TryGetTarget(out inner))
                {
                    inner.OnPropertyChanged(property, oldValue);
                }
                else
                {
                    Handle?.Dispose();
                    Handle = null;
                }
            }

            public void OnRegistered(IObservableProperty property)
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));

                IObserver inner;

                if (_innerObserver.TryGetTarget(out inner))
                {
                    inner.OnRegistered(property);
                }
                else
                {
                    Handle?.Dispose();
                    Handle = null;
                }
            }

            public void OnUnregistered(IObservableProperty property)
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));

                IObserver inner;

                if (_innerObserver.TryGetTarget(out inner))
                {
                    inner.OnUnregistered(property);
                }
                else
                {
                    Handle?.Dispose();
                    Handle = null;
                }
            }

            public void OnRegistered(IObservableObject instance)
            {
                IObserver inner;

                if (_innerObserver.TryGetTarget(out inner))
                {
                    inner.OnRegistered(instance);
                }
                else
                {
                    Handle?.Dispose();
                    Handle = null;
                }
            }

            public void OnUnregistered(IObservableObject instance)
            {

                IObserver inner;

                if (_innerObserver.TryGetTarget(out inner))
                {
                    inner.OnUnregistered(instance);
                }
                else
                {
                    Handle?.Dispose();
                    Handle = null;
                }
            }
        }

        private sealed class WeakObserver<T> : Observer<T>
        {
            private WeakReference<IObserver<T>> _innerObserver;

            public IDisposable Handle { get; private set; }

            public WeakObserver(IObserver<T> innerObserver, IObservableProperty<T> observableProperty)
            {
                if (innerObserver == null)
                    throw new ArgumentNullException(nameof(innerObserver));
                if (observableProperty == null)
                    throw new ArgumentNullException(nameof(observableProperty));

                _innerObserver = new WeakReference<IObserver<T>>(innerObserver);
                Handle = observableProperty.RegisterObserver(this);

                CreateDisposable();
            }

            private void CreateDisposable()
            {
                IDisposable p = Handle;

                Handle = Disposable.Create(() =>
                {
                    p?.Dispose();
                    p = null;
                    _innerObserver = null;
                });
            }

            public override void OnPropertyChanged(IObservableProperty<T> property, T oldValue)
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));

                IObserver<T> inner;

                if (_innerObserver.TryGetTarget(out inner))
                {
                    inner.OnPropertyChanged(property, oldValue);
                }
                else
                {
                    Handle?.Dispose();
                    Handle = null;
                }
            }

            public override void OnRegistered(IObservableProperty<T> property)
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));

                IObserver<T> inner;

                if (_innerObserver.TryGetTarget(out inner))
                {
                    inner.OnRegistered(property);
                }
                else
                {
                    Handle?.Dispose();
                    Handle = null;
                }
            }

            public override void OnUnregistered(IObservableProperty<T> property)
            {
                if (property == null)
                    throw new ArgumentNullException(nameof(property));

                IObserver<T> inner;

                if (_innerObserver.TryGetTarget(out inner))
                {
                    inner.OnUnregistered(property);
                }
                else
                {
                    Handle?.Dispose();
                    Handle = null;
                }
            }
        }
    }
}
