using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility
{
    public static class SerializationInfoExtensions
    {
        public static T GetValue<T>(this SerializationInfo instance, string name)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return (T)instance.GetValue(name, typeof(T));
        }

        public static bool TryGetValue<T>(this SerializationInfo instance, string name, out T value)
        {
            try
            {
                value = GetValue<T>(instance, name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(T);
                return false;
            }
        }

        public static bool TryGetBoolean(this SerializationInfo instance, string name, out bool value)
        {
            try
            {
                value = instance.GetBoolean(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(bool);
                return false;
            }
        }

        public static bool TryGetByte(this SerializationInfo instance, string name, out byte value)
        {
            try
            {
                value = instance.GetByte(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(byte);
                return false;
            }
        }

        public static bool TryGetChar(this SerializationInfo instance, string name, out char value)
        {
            try
            {
                value = instance.GetChar(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(char);
                return false;
            }
        }

        public static bool TryGetDateTime(this SerializationInfo instance, string name, out DateTime value)
        {
            try
            {
                value = instance.GetDateTime(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(DateTime);
                return false;
            }
        }

        public static bool TryGetDecimal(this SerializationInfo instance, string name, out decimal value)
        {
            try
            {
                value = instance.GetDecimal(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(decimal);
                return false;
            }
        }

        public static bool TryGetDouble(this SerializationInfo instance, string name, out double value)
        {
            try
            {
                value = instance.GetDouble(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(double);
                return false;
            }
        }

        public static bool TryGetInt16(this SerializationInfo instance, string name, out short value)
        {
            try
            {
                value = instance.GetInt16(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(short);
                return false;
            }
        }

        public static bool TryGetInt32(this SerializationInfo instance, string name, out int value)
        {
            try
            {
                value = instance.GetInt32(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(int);
                return false;
            }
        }

        public static bool TryGetInt64(this SerializationInfo instance, string name, out long value)
        {
            try
            {
                value = instance.GetInt64(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(long);
                return false;
            }
        }

        public static bool TryGetSByte(this SerializationInfo instance, string name, out sbyte value)
        {
            try
            {
                value = instance.GetSByte(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(sbyte);
                return false;
            }
        }

        public static bool TryGetSingle(this SerializationInfo instance, string name, out float value)
        {
            try
            {
                value = instance.GetSingle(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(float);
                return false;
            }
        }

        public static bool TryGetString(this SerializationInfo instance, string name, out string value)
        {
            try
            {
                value = instance.GetString(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(string);
                return false;
            }
        }

        public static string TryGetString(this SerializationInfo instance, string name)
        {
            string v;

            if (!TryGetString(instance, name, out v))
                v = null;

            return v;
        }

        public static bool TryGetUInt16(this SerializationInfo instance, string name, out ushort value)
        {
            try
            {
                value = instance.GetUInt16(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(ushort);
                return false;
            }
        }

        public static bool TryGetUInt32(this SerializationInfo instance, string name, out uint value)
        {
            try
            {
                value = instance.GetUInt32(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(uint);
                return false;
            }
        }

        public static bool TryGetUInt64(this SerializationInfo instance, string name, out ulong value)
        {
            try
            {
                value = instance.GetUInt64(name);
                return true;
            }
            catch (SerializationException)
            {
                value = default(ulong);
                return false;
            }
        }

        public static bool? TryGetBoolean(this SerializationInfo instance, string name)
        {
            if (!TryGetBoolean(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static byte? TryGetByte(this SerializationInfo instance, string name)
        {
            if (!TryGetByte(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static char? TryGetChar(this SerializationInfo instance, string name)
        {
            if (!TryGetChar(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static DateTime? TryGetDateTime(this SerializationInfo instance, string name)
        {
            if (!TryGetDateTime(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static decimal? TryGetDecimal(this SerializationInfo instance, string name)
        {
            if (!TryGetDecimal(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static double? TryGetDouble(this SerializationInfo instance, string name)
        {
            if (!TryGetDouble(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static short? TryGetInt16(this SerializationInfo instance, string name)
        {
            if (!TryGetInt16(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static int? TryGetInt32(this SerializationInfo instance, string name)
        {
            if (!TryGetInt32(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static long? TryGetInt64(this SerializationInfo instance, string name)
        {
            if (!TryGetInt64(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static sbyte? TryGetSByte(this SerializationInfo instance, string name)
        {
            if (!TryGetSByte(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static float? TryGetSingle(this SerializationInfo instance, string name)
        {
            if (!TryGetSingle(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static ushort? TryGetUInt16(this SerializationInfo instance, string name)
        {
            if (!TryGetUInt16(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static uint? TryGetUInt32(this SerializationInfo instance, string name)
        {
            if (!TryGetUInt32(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }

        public static ulong? TryGetUInt64(this SerializationInfo instance, string name)
        {
            if (!TryGetUInt64(instance, name, out var v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }
    }
}
