using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utility.Contexts
{
    public static class Contexts
    {
        public static TContext GetCurrentContext<TContext>()
            where TContext : class
        {
            return ThreadContextual<TContext>.CurrentContext;
        }

        public static TContext GetCurrentContext<TContext>(bool fallbackToGlobal)
            where TContext : class
        {
            return ThreadContextual<TContext>.GetCurrentContext(fallbackToGlobal);
        }

        public static object GetCurrentContext(Type contextType)
        {
            if (contextType == null)
                throw new ArgumentNullException(nameof(contextType));

            return typeof(ThreadContextual<>).MakeGenericType(contextType).InvokeMember(
                "CurrentContext",
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static, // | BindingFlags.DoNotWrapExceptions,
                null,
                null,
                null);
        }

        public static object GetCurrentContext(Type contextType, bool fallbackToGlobal)
        {
            if (contextType == null)
                throw new ArgumentNullException(nameof(contextType));

            return typeof(ThreadContextual<>).MakeGenericType(contextType).InvokeMember(
                "GetThreadContext",
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static | BindingFlags.DoNotWrapExceptions,
                null,
                null,
                new object[] { fallbackToGlobal });
        }

        public static TContext GetGlobalContext<TContext>()
            where TContext : class
        {
            return GlobalContext<TContext>.CurrentContext;
        }

        public static object GetGlobalContext(Type contextType)
        {
            if (contextType == null)
                throw new ArgumentNullException(nameof(contextType));

            return typeof(GlobalContext<>).MakeGenericType(contextType).InvokeMember(
                "CurrentContext",
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static | BindingFlags.DoNotWrapExceptions,
                null,
                null,
                null);
        }

        public static TContext GetGlobalContext<TContext>(bool fallbackToDefault)
            where TContext : class
        {
            return GlobalContext<TContext>.GetCurrentContext(fallbackToDefault);
        }

        public static object GetGlobalContext(Type contextType, bool fallbackToDefault)
        {
            if (contextType == null)
                throw new ArgumentNullException(nameof(contextType));

            return typeof(GlobalContext<>).MakeGenericType(contextType).InvokeMember(
                "GetCurrentContext",
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static | BindingFlags.DoNotWrapExceptions,
                null,
                null,
                new object[] { fallbackToDefault });
        }
    }
}
