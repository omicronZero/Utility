using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ObjectDescription
{
    public class CompositeObserver : IObserver, ICollection<IObserver>, IReadOnlyCollection<IObserver>
    {
        private readonly DisposableList<IObserver> _observers;
        private readonly bool _parallelized;

        public CompositeObserver()
            : this(false)
        { }

        public CompositeObserver(bool parallelized)
        {
            _observers = new DisposableList<IObserver>();
            _parallelized = parallelized;
        }

        public int Count => _observers.Count;

        public IDisposable AddObserver(IObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            return _observers.Add(observer);
        }

        public void OnPropertyChanged(IObservableProperty property, object oldValue)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnPropertyChanged(property, oldValue));
            }
            else
            {
                foreach (IObserver s in _observers)
                    s.OnPropertyChanged(property, oldValue);
            }
        }

        public void OnRegistered(IObservableProperty property)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnRegistered(property));
            }
            else
            {
                foreach (IObserver s in _observers)
                    s.OnRegistered(property);
            }
        }

        public void OnUnregistered(IObservableProperty property)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnUnregistered(property));
            }
            else
            {
                foreach (IObserver s in _observers)
                    s.OnUnregistered(property);
            }
        }

        public void OnRegistered(IObservableObject instance)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnRegistered(instance));
            }
            else
            {
                foreach (IObserver s in _observers)
                    s.OnRegistered(instance);
            }
        }

        public void OnUnregistered(IObservableObject instance)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnUnregistered(instance));
            }
            else
            {
                foreach (IObserver s in _observers)
                    s.OnUnregistered(instance);
            }
        }

        bool ICollection<IObserver>.IsReadOnly => true;

        void ICollection<IObserver>.Add(IObserver item)
        {
            AddObserver(item);
        }

        void ICollection<IObserver>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<IObserver>.Contains(IObserver item)
        {
            return _observers.Contains(item);
        }

        void ICollection<IObserver>.CopyTo(IObserver[] array, int arrayIndex)
        {
            _observers.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IObserver> GetEnumerator()
        {
            return _observers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<IObserver>.Remove(IObserver item)
        {
            throw new NotSupportedException();
        }
    }

    public class CompositeObserver<T> : IObserver<T>, ICollection<IObserver<T>>, IReadOnlyCollection<IObserver<T>>
    {
        private readonly DisposableList<IObserver<T>> _observers;
        private readonly bool _parallelized;

        public CompositeObserver()
            : this(false)
        { }

        public CompositeObserver(bool parallelized)
        {
            _observers = new DisposableList<IObserver<T>>();
            _parallelized = parallelized;
        }

        public int Count => _observers.Count;

        public IDisposable AddObserver(IObserver<T> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            return _observers.Add(observer);
        }

        public void OnPropertyChanged(IObservableProperty<T> property, T oldValue)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnPropertyChanged(property, oldValue));
            }
            else
            {
                foreach (IObserver<T> s in _observers)
                    s.OnPropertyChanged(property, oldValue);
            }
        }

        void IObserver.OnPropertyChanged(IObservableProperty property, object oldValue)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnPropertyChanged(property, oldValue));
            }
            else
            {
                foreach (IObserver<T> s in _observers)
                    s.OnPropertyChanged(property, oldValue);
            }
        }

        public void OnRegistered(IObservableProperty<T> property)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnRegistered(property));
            }
            else
            {
                foreach (IObserver<T> s in _observers)
                    s.OnRegistered(property);
            }
        }

        public void OnUnregistered(IObservableProperty<T> property)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnUnregistered(property));
            }
            else
            {
                foreach (IObserver<T> s in _observers)
                    s.OnUnregistered(property);
            }
        }

        void IObserver.OnRegistered(IObservableProperty property)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnRegistered(property));
            }
            else
            {
                foreach (IObserver<T> s in _observers)
                    s.OnRegistered(property);
            }
        }

        void IObserver.OnUnregistered(IObservableProperty property)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnUnregistered(property));
            }
            else
            {
                foreach (IObserver<T> s in _observers)
                    s.OnUnregistered(property);
            }
        }

        void IObserver.OnRegistered(IObservableObject instance)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnRegistered(instance));
            }
            else
            {
                foreach (IObserver s in _observers)
                    s.OnRegistered(instance);
            }
        }

        void IObserver.OnUnregistered(IObservableObject instance)
        {
            if (_parallelized)
            {
                Parallel.ForEach(_observers, (s) => s.OnUnregistered(instance));
            }
            else
            {
                foreach (IObserver s in _observers)
                    s.OnUnregistered(instance);
            }
        }

        bool ICollection<IObserver<T>>.IsReadOnly => true;

        void ICollection<IObserver<T>>.Add(IObserver<T> item)
        {
            AddObserver(item);
        }

        void ICollection<IObserver<T>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<IObserver<T>>.Contains(IObserver<T> item)
        {
            return _observers.Contains(item);
        }

        void ICollection<IObserver<T>>.CopyTo(IObserver<T>[] array, int arrayIndex)
        {
            _observers.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IObserver<T>> GetEnumerator()
        {
            return _observers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<IObserver<T>>.Remove(IObserver<T> item)
        {
            throw new NotSupportedException();
        }
    }
}
