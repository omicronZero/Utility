using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utility.Data
{
    public sealed class PrimitiveTypeFormatter : TypeFormatter<EmptyType, EmptyType>
    {
        public object Read(Stream stream, Type valueType)
        {
            return Read(stream, valueType, EmptyType.Instance);
        }

        public override object Read(Stream stream, Type valueType, EmptyType parameter)
        {
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));

            if (valueType.IsPrimitive)
            {
                DataStream s = DataStream.AsDataStream(stream);

                if (valueType == typeof(char))
                    return s.ReadAssembledSingletonComplete(s.ReadChar());
                else if (valueType == typeof(sbyte))
                    return s.ReadAssembledSingletonComplete(s.ReadSByte());
                else if (valueType == typeof(byte))
                    return s.ReadAssembledSingletonComplete(s.ReadOneByte());
                else if (valueType == typeof(short))
                    return s.ReadAssembledSingletonComplete(s.ReadInt16());
                else if (valueType == typeof(ushort))
                    return s.ReadAssembledSingletonComplete(s.ReadUInt16());
                else if (valueType == typeof(int))
                    return s.ReadAssembledSingletonComplete(s.ReadInt32());
                else if (valueType == typeof(uint))
                    return s.ReadAssembledSingletonComplete(s.ReadUInt32());
                else if (valueType == typeof(long))
                    return s.ReadAssembledSingletonComplete(s.ReadInt64());
                else if (valueType == typeof(ulong))
                    return s.ReadAssembledSingletonComplete(s.ReadUInt64());
                else if (valueType == typeof(float))
                    return s.ReadAssembledSingletonComplete(s.ReadSingle());
                else if (valueType == typeof(double))
                    return s.ReadAssembledSingletonComplete(s.ReadDouble());
            }

            throw new NotSupportedException("Unsupported object type.");
        }

        public override bool SupportsType(Type type)
        {
            if (type == null)
                return false;

            return type.IsPrimitive;
        }

        public void Write(Stream stream, object value)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Write(stream, value, value.GetType());
        }

        public void Write(Stream stream, object value, Type valueType)
        {
            Write(stream, value, valueType, EmptyType.Instance);
        }

        public override void Write(Stream stream, object value, Type valueType, EmptyType parameter)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));

            Type objectType = value.GetType();

            if (objectType.IsPrimitive && valueType == objectType)
            {
                DataStream s = DataStream.AsDataStream(stream);

                if (objectType == typeof(char))
                    s.Write((char)value);
                else if (objectType == typeof(sbyte))
                    s.Write((sbyte)value);
                else if (objectType == typeof(byte))
                    s.Write((byte)value);
                else if (objectType == typeof(short))
                    s.Write((short)value);
                else if (objectType == typeof(ushort))
                    s.Write((ushort)value);
                else if (objectType == typeof(int))
                    s.Write((int)value);
                else if (objectType == typeof(uint))
                    s.Write((uint)value);
                else if (objectType == typeof(long))
                    s.Write((long)value);
                else if (objectType == typeof(ulong))
                    s.Write((ulong)value);
                else if (objectType == typeof(float))
                    s.Write((float)value);
                else if (objectType == typeof(double))
                    s.Write((double)value);
            }

            throw new NotSupportedException("Unsupported object type.");
        }
    }
}
