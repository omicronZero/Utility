using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Utility.Dynamic
{
    public static class DelegateHandler
    {
        private static long _delegateCounter;

        private static readonly ConcurrentDictionary<TypeArrayKey, Type> _delegateTypes;

        static DelegateHandler()
        {
            _delegateTypes = new ConcurrentDictionary<TypeArrayKey, Type>();
        }

        public static Type CreateDynamicDelegate(Type returnType, params Type[] parameterTypes)
        {
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            if (parameterTypes == null)
                throw new ArgumentNullException(nameof(parameterTypes));

            //TODO: fallback to generic types

            return _delegateTypes.GetOrAdd(new TypeArrayKey(returnType, parameterTypes), (tp) =>
            {
                long c = System.Threading.Interlocked.Increment(ref _delegateCounter);

                TypeBuilder builder = DynamicHelper.GetAssociatedModuleBuilder<Key>().DefineType("DynamicDelegate_" + Convert.ToString(c, 16), TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass, typeof(MulticastDelegate));

                var ctor = builder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                     CallingConventions.Standard,
                     new Type[] { typeof(object), typeof(IntPtr) });

                var invoke = builder.DefineMethod("Invoke",
                     MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.PrivateScope | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                     CallingConventions.HasThis,
                     returnType,
                     parameterTypes);

                var beginInvoke = builder.DefineMethod("BeginInvoke",
                     MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.PrivateScope | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                     CallingConventions.HasThis,
                     typeof(AsyncCallback),
                     parameterTypes.Concat(new Type[] { typeof(AsyncCallback), typeof(object) }).ToArray());

                var endInvoke = builder.DefineMethod("EndInvoke",
                     MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.PrivateScope | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                     CallingConventions.HasThis,
                     typeof(void),
                     new Type[] { typeof(IAsyncResult) });

                ctor.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.Runtime);
                invoke.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.Runtime);
                beginInvoke.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.Runtime);
                endInvoke.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.Runtime);

                return builder.CreateTypeInfo();
            });
        }

        public static MethodInfo CreateStaticCall(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (method.IsStatic)
                throw new ArgumentException("The specified method is already static.", nameof(method));

            ParameterInfo[] param = method.GetParameters();

            TypeBuilder typeBuilder = DynamicMethod.NextType();

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name,
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                method.ReturnType,
                new Type[] { method.DeclaringType }.Concat(param.Select((s) => s.ParameterType)).ToArray());

            ILGenerator ilg = methodBuilder.GetILGenerator();

            ilg.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < param.Length; i++)
            {
                ilg.Emit(OpCodes.Ldarg, i + 1);
            }

            ilg.Emit(OpCodes.Call, method);
            ilg.Emit(OpCodes.Ret);

            return method;
        }

        private sealed class Key { }
    }
}
