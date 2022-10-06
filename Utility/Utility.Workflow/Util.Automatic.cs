
using System;
using System.Collections.Generic;

namespace Utility
{
	static partial class Util
	{

        public static TReturn CallSynchronized<TReturn>(object syncObject, Func<TReturn> func)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    return func();
                }
            }
            else
            {
                return func();
            }
        }

        public static void CallSynchronized(object syncObject, Action handler)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    handler();
                }
            }
            else
            {
                handler();
            }
        }
		
        public static TReturn CallSynchronized<T1, TReturn>(object syncObject, T1 param1, Func<T1, TReturn> func)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    return func(param1);
                }
            }
            else
            {
                return func(param1);
            }
        }

        public static void CallSynchronized<T1>(object syncObject, T1 param1, Action<T1> handler)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    handler(param1);
                }
            }
            else
            {
                handler(param1);
            }
        }
		
        public static TReturn CallSynchronized<T1, T2, TReturn>(object syncObject, T1 param1, T2 param2, Func<T1, T2, TReturn> func)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    return func(param1, param2);
                }
            }
            else
            {
                return func(param1, param2);
            }
        }

        public static void CallSynchronized<T1, T2>(object syncObject, T1 param1, T2 param2, Action<T1, T2> handler)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    handler(param1, param2);
                }
            }
            else
            {
                handler(param1, param2);
            }
        }
		
        public static TReturn CallSynchronized<T1, T2, T3, TReturn>(object syncObject, T1 param1, T2 param2, T3 param3, Func<T1, T2, T3, TReturn> func)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    return func(param1, param2, param3);
                }
            }
            else
            {
                return func(param1, param2, param3);
            }
        }

        public static void CallSynchronized<T1, T2, T3>(object syncObject, T1 param1, T2 param2, T3 param3, Action<T1, T2, T3> handler)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    handler(param1, param2, param3);
                }
            }
            else
            {
                handler(param1, param2, param3);
            }
        }
		
        public static TReturn CallSynchronized<T1, T2, T3, T4, TReturn>(object syncObject, T1 param1, T2 param2, T3 param3, T4 param4, Func<T1, T2, T3, T4, TReturn> func)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    return func(param1, param2, param3, param4);
                }
            }
            else
            {
                return func(param1, param2, param3, param4);
            }
        }

        public static void CallSynchronized<T1, T2, T3, T4>(object syncObject, T1 param1, T2 param2, T3 param3, T4 param4, Action<T1, T2, T3, T4> handler)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    handler(param1, param2, param3, param4);
                }
            }
            else
            {
                handler(param1, param2, param3, param4);
            }
        }
		
        public static TReturn CallSynchronized<T1, T2, T3, T4, T5, TReturn>(object syncObject, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Func<T1, T2, T3, T4, T5, TReturn> func)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    return func(param1, param2, param3, param4, param5);
                }
            }
            else
            {
                return func(param1, param2, param3, param4, param5);
            }
        }

        public static void CallSynchronized<T1, T2, T3, T4, T5>(object syncObject, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Action<T1, T2, T3, T4, T5> handler)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    handler(param1, param2, param3, param4, param5);
                }
            }
            else
            {
                handler(param1, param2, param3, param4, param5);
            }
        }
		
        public static TReturn CallSynchronized<T1, T2, T3, T4, T5, T6, TReturn>(object syncObject, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, Func<T1, T2, T3, T4, T5, T6, TReturn> func)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    return func(param1, param2, param3, param4, param5, param6);
                }
            }
            else
            {
                return func(param1, param2, param3, param4, param5, param6);
            }
        }

        public static void CallSynchronized<T1, T2, T3, T4, T5, T6>(object syncObject, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, Action<T1, T2, T3, T4, T5, T6> handler)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    handler(param1, param2, param3, param4, param5, param6);
                }
            }
            else
            {
                handler(param1, param2, param3, param4, param5, param6);
            }
        }
		
        public static TReturn CallSynchronized<T1, T2, T3, T4, T5, T6, T7, TReturn>(object syncObject, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, Func<T1, T2, T3, T4, T5, T6, T7, TReturn> func)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    return func(param1, param2, param3, param4, param5, param6, param7);
                }
            }
            else
            {
                return func(param1, param2, param3, param4, param5, param6, param7);
            }
        }

        public static void CallSynchronized<T1, T2, T3, T4, T5, T6, T7>(object syncObject, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, Action<T1, T2, T3, T4, T5, T6, T7> handler)
        {
            if (syncObject != null)
            {
                lock (syncObject)
                {
                    handler(param1, param2, param3, param4, param5, param6, param7);
                }
            }
            else
            {
                handler(param1, param2, param3, param4, param5, param6, param7);
            }
        }
			}
}