using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Utility.Dynamic
{
    internal static class DynamicMethod
    {
        private static readonly object _syncRoot;

        private static ModuleBuilder _moduleBuilder;

        private static long _typeId;

        static DynamicMethod()
        {
            _syncRoot = new object();
        }

        private static ModuleBuilder ModuleBuilder
        {
            get
            {
                if (_moduleBuilder == null)
                {
                    lock (_syncRoot)
                    {
                        if (_moduleBuilder == null)
                        {
                            _moduleBuilder = DynamicHelper.GetAssociatedModuleBuilder<TypeKey>();
                        }
                    }
                }

                return _moduleBuilder;
            }
        }

        public static TypeBuilder NextType()
        {
           return ModuleBuilder.DefineType("Dynamic" + Convert.ToString(System.Threading.Interlocked.Increment(ref _typeId)), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);
        }

        public static MethodBuilder Create(string methodName, Type returnType, Type[] parameterTypes)
        {
            return NextType().DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.Final, CallingConventions.Standard, returnType, parameterTypes);
        }

        public static MethodBuilder Create(
            string methodName, 
            Type returnType,
            Type[] returnTypeRequiredModifiers,
            Type[] returnTypeOptionalModifiers, 
            Type[] parameterTypes,
            Type[][] parameterTypesRequiredCustomModifiers, 
            Type[][] parameterTypesOptionalCustomModifiers)
        {
            return NextType().DefineMethod(methodName,
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.Final, 
                CallingConventions.Standard, 
                returnType,
                returnTypeRequiredModifiers, 
                returnTypeOptionalModifiers,
                parameterTypes, 
                parameterTypesRequiredCustomModifiers, parameterTypesOptionalCustomModifiers);
        }

        private sealed class TypeKey { }
    }
}
