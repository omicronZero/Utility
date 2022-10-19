using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utility.Serialization.Serializers
{
    public abstract class LateObject
    {
        public object? Value => ValueCore;

        public abstract event EventHandler ValueChanged;
        internal abstract object? ValueCore { get; }
        public abstract bool IsValueSet { get; }
        public abstract Type ValueType { get; }

        private protected LateObject()
        { }
    }

    public sealed class LateObject<T> : LateObject
    {
        private EventHandler? _valueChanged;
        private T _value;
        private bool _isValueSet;

        private readonly SpinLock _syncObject;

        public override bool IsValueSet => _isValueSet;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public LateObject(out Action<T> setter)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            setter = (value) =>
            {
                using (Lock())
                {
                    if (IsValueSet)
                        throw new InvalidOperationException("The value has already been set. The instance is immutable.");

                    _isValueSet = true;

                    _value = value;
                }

                _valueChanged?.Invoke(this, EventArgs.Empty);
                _valueChanged = null;
            };
        }

        public override event EventHandler ValueChanged
        {
            add
            {
                //we directly invoke value listeners if the value is already computed
                using (Lock())
                {
                    if (IsValueSet)
                        value.Invoke(this, EventArgs.Empty);
                    else
                        _valueChanged += value;
                }
            }
            remove
            {
                _valueChanged -= value;
            }
        }

        private LockHandle Lock() => new LockHandle(this);

        new public T Value
        {
            get
            {
                if (!IsValueSet)
                    throw new InvalidOperationException("The value has not been set.");

                return _value;
            }
        }

        internal sealed override object? ValueCore => Value;
        public sealed override Type ValueType => typeof(T);

        private struct LockHandle : IDisposable
        {
            private readonly LateObject<T> _instance;
            private bool _captured;

            public LockHandle(LateObject<T> instance)
            {
                _captured = false;
                _instance = instance;
                instance._syncObject.Enter(ref _captured);
            }

            public void Dispose()
            {
                if (_captured)
                {
                    _captured = false;
                    _instance._syncObject.Exit();
                }
            }
        }
    }
}
