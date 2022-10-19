using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Utility.Serialization
{
    public static class TypeExtensions
    {
        public static bool IsUnmanaged(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsPointer)
                return true;

            if (type.IsByRef)
                return false;

            //first, we retrieve the IsUnmanaged method using a dummy delegate, then we invoke it
            return (bool)((Func<bool>)IsUnmanaged<int>).Method
                .GetGenericMethodDefinition()
                .MakeGenericMethod(type)
                .Invoke(null, null)!;
        }

        public static bool IsUnmanaged<T>() => Helper<T>.IsUnmanaged;

        public static int? UnmanagedSize(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsUnmanaged())
                return null;

            return (int)typeof(UnmanagedHelper<>).MakeGenericType(type)
                .GetMethod("GetUnmanagedSize", BindingFlags.Public | BindingFlags.Static)!
                .Invoke(null, null)!;
        }

        public static int? UnmanagedSize<T>()
            => Helper<T>.UnmanagedSize;


        public static IEnumerable<Type> GetImplementationOf(this Type type, Type baseType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (baseType == null)
                throw new ArgumentNullException(nameof(baseType));

            if (type.IsEnum)
            {
                if (type == baseType)
                    yield return type;

                type = type.GetEnumUnderlyingType();
            }

            if (!baseType.IsGenericTypeDefinition)
            {
                if (baseType.IsAssignableFrom(type))
                    yield return baseType;

                yield break;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
            {
                yield return type;
                yield break;
            }

            if (baseType.IsInterface)
            {
                foreach (Type bt in type.GetInterfaces())
                {
                    if (!bt.IsGenericType)
                        continue;

                    if (bt.GetGenericTypeDefinition() == baseType)
                        yield return bt;
                }
            }
            else
            {
                var bt = type.BaseType;
                bool isTypeGeneric = type.IsGenericTypeDefinition;

                while (bt != null)
                {
                    //BaseType does not yield the generic base type but rather it is bound to the
                    //type parameters of its child
                    //--> we have to retrieve the definition if the original type was a generic type definition
                    if (isTypeGeneric)
                        bt = bt.GetGenericTypeDefinition();

                    if (bt.IsGenericType && bt.GetGenericTypeDefinition() == baseType)
                    {
                        yield return bt;
                        yield break;
                    }

                    bt = bt.BaseType;
                }
            }
        }

        private static class Helper<T>
        {
            public static bool IsUnmanaged { get; }
            public static int? UnmanagedSize { get; }

            static Helper()
            {
                IsUnmanaged = GetUnmanaged();
                UnmanagedSize = TypeExtensions.UnmanagedSize(typeof(T));
            }

            private static bool GetUnmanaged()
            {
                var type = typeof(T);

                if (type.IsByRef)
                    return false;

                if (type.IsPointer)
                    return true;

                return !(bool)((Func<bool>)RuntimeHelpers.IsReferenceOrContainsReferences<int>)
                     .Method.GetGenericMethodDefinition()
                     .MakeGenericMethod(type)
                     .Invoke(null, null)!;
            }
        }

        private static unsafe class UnmanagedHelper<T>
            where T : unmanaged
        {
            public static int GetUnmanagedSize() => sizeof(T);
        }
    }
}
