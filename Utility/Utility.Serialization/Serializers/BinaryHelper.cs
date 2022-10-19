using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Serialization.Serializers
{
    internal static class BinaryHelper<T>
    {
        private static readonly Action<byte[], int, T>? _getDataImpl;
        private static readonly Func<byte[], int, T>? _getObjectImpl;

        static BinaryHelper()
        {
            Type? type;
            try
            {
                type = typeof(Helper<>).MakeGenericType(typeof(T), typeof(T));
            }
            catch (TypeInitializationException)
            {
                type = null;
            }

            if (type != null)
            {
                _getDataImpl = (Action<byte[], int, T>)type.GetMethod("GetData", BindingFlags.Public | BindingFlags.Static)!.CreateDelegate(typeof(Action<byte[], int, T>));
                _getObjectImpl = (Func<byte[], int, T>)type.GetMethod("GetObject", BindingFlags.Public | BindingFlags.Static)!.CreateDelegate(typeof(Func<byte[], int, T>));
            }
        }

        public static void GetData(byte[] buffer, int index, T instance)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (_getDataImpl == null)
                throw new ArgumentException("Unsupported type: The instance's type is not an unmanaged type.", nameof(instance));

            _getDataImpl(buffer, index, instance);
        }

        public static T GetObject(byte[] buffer, int index)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));


            if (_getObjectImpl == null)
                throw new ArgumentException("Unsupported type: The target's type is not an unmanaged type.");

            return _getObjectImpl(buffer, index);
        }

        private static unsafe class Helper<TUnmanaged>
            where TUnmanaged : unmanaged
        {
            public static void GetData(byte[] buffer, int index, TUnmanaged instance)
            {
                if (buffer.Length < sizeof(TUnmanaged))
                    throw new ArgumentException("Buffer does not have sufficient capacity.");

                Marshal.Copy(new IntPtr(&instance), buffer, index, sizeof(TUnmanaged));
            }

            public static TUnmanaged GetObject(byte[] buffer, int index)
            {
                if (buffer.Length < sizeof(TUnmanaged))
                    throw new ArgumentException("Buffer does not have sufficient capacity.");

                TUnmanaged inst = default;

                Marshal.Copy(buffer, index, new IntPtr(&inst), sizeof(TUnmanaged));

                return inst;
            }
        }
    }
}
