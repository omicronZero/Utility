using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Utility.Data
{
    public abstract class TypeFormatter
    {
        public abstract Type InParameterType { get; }
        public abstract Type OutParameterType { get; }

        public abstract void Write(Stream stream, object value, Type valueType, object parameters);
        public abstract object Read(Stream stream, Type valueType, object parameters);

        public abstract bool SupportsType(Type type);

        public virtual T Read<T>(Stream stream, object parameters)
        {
            return (T)Read(stream, typeof(T), parameters);
        }

        public virtual void Write<T>(Stream stream, T value, object parameters)
        {
            Write(stream, value, typeof(T), parameters);
        }
    }

    public abstract class TypeFormatter<TInParameters, TOutParameters> : TypeFormatter
    {
        public abstract void Write(Stream stream, object value, Type valueType, TOutParameters parameters);
        public abstract object Read(Stream stream, Type valueType, TInParameters parameters);

        public virtual T Read<T>(Stream stream, TInParameters parameters)
        {
            return (T)Read(stream, typeof(T), parameters);
        }

        public virtual void Write<T>(Stream stream, T value, TOutParameters parameters)
        {
            Write(stream, value, typeof(T), parameters);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Type InParameterType => typeof(TInParameters);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Type OutParameterType => typeof(TOutParameters);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Write(Stream stream, object value, Type valueType, object parameters)
        {
            Write(stream, value, valueType, CastParameter<TOutParameters>(parameters));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object Read(Stream stream, Type valueType, object parameters)
        {
            return Read(stream, valueType, CastParameter<TInParameters>(parameters));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override T Read<T>(Stream stream, object parameters)
        {
            return Read<T>(stream, CastParameter<TInParameters>(parameters));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Write<T>(Stream stream, T value, object parameters)
        {
            Write<T>(stream, value, CastParameter<TOutParameters>(parameters));
        }

        private static T CastParameter<T>(object parameters)
        {
            if (parameters == null)
            {
                if (typeof(T).IsValueType)
                    throw new ArgumentNullException(nameof(parameters));
            }
            else if (!(parameters is T))
                throw new ArgumentException($"Parameters must be of type { typeof(T).FullName }.");

            return (T)parameters;
        }
    }
}
