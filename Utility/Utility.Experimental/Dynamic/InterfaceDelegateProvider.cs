using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Utility.Dynamic
{
    /// <summary>
    /// Provides dynamic types that allow the delegation of calls on interfaces to a delegation unit.
    /// This class cannot be inherited.
    /// </summary>
    public static class InterfaceDelegateProvider
    {
        //TODO: add support for in/out generics
        private const string InterfaceDelegateTypeName = "InterfaceDelegate";
        private const string InterfaceBaseName = "InterfaceBaseName";
        private const string InterfaceBaseTypeName = "_interfaceBaseType";

        private static readonly ConcurrentDictionary<(Type InterfaceType, bool MarshalByRef), Type> _interfaceMapping;

        static InterfaceDelegateProvider()
        {
            _interfaceMapping = new ConcurrentDictionary<(Type, bool), Type>();
        }

        private static string GetDelegatedTypeName(Type interfaceType)
        {
            //generate unique name for the specified type
            string baseName = InterfaceBaseName + DynamicHelper.GetMd5Name(interfaceType.FullName) + "_";

            StringBuilder sb = new StringBuilder();

            sb.Append(InterfaceBaseName);
            DynamicHelper.GetMd5Name(interfaceType.FullName, sb);
            sb.Append('_');
            sb.Append(Convert.ToString((long)interfaceType.MetadataToken | ((long)interfaceType.Module.MetadataToken) << 32, 16));

            return baseName;
        }

        private static Type GetInterfaceDelegateType(Type interfaceType, bool marshalByRef)
        {
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));
            if (!interfaceType.IsInterface)
                throw new ArgumentException("Interface type expected.", nameof(interfaceType));

            if (interfaceType.IsGenericType && !interfaceType.IsGenericTypeDefinition)
                return GetInterfaceDelegateType(interfaceType.GetGenericTypeDefinition(), marshalByRef).MakeGenericType(interfaceType.GetGenericArguments());

            return _interfaceMapping.GetOrAdd((interfaceType, marshalByRef), (t) => GetInterfaceDelegateCore(t.InterfaceType, t.MarshalByRef));
        }

        private static Type GetInterfaceDelegateCore(Type interfaceType, bool marshalByRef)
        {
            Type delegateType;
            string typeName = GetDelegatedTypeName(interfaceType);


            //define container type to store delegation type as a nested type (to provide static method-Array for generic methods)
            //the container stores information about methods to prevent unnecessary redundance in case of generic interface requests
            TypeBuilder container = DynamicHelper.GetAssociatedModuleBuilder<KeyType>().DefineType(typeName, TypeAttributes.Public | TypeAttributes.Sealed, typeof(object));
            Type containerType;
            Type baseType = marshalByRef ? typeof(MarshalByRefObject) : typeof(object);
            TypeBuilder delegationUnitBuilder = DeclareType(container, InterfaceDelegateTypeName, interfaceType, baseType);
            ILGenerator generator;
            ICollection<MemberInfo> members = GetMembers(interfaceType);
            Dictionary<MethodInfo, MethodBuilder> methods = new Dictionary<MethodInfo, MethodBuilder>();
            MethodInfo[] methodArray;
            ConstructorInfo baseTypeConstructor = baseType.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            if (baseTypeConstructor == null || !(baseTypeConstructor.IsPublic || baseTypeConstructor.IsFamily))
                throw new InvalidOperationException("Base type constructor inaccessible.");

            MethodInfo typeFromHandleMethod = typeof(Type).GetMethod(
                "GetTypeFromHandle",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new Type[] { typeof(RuntimeTypeHandle) },
                null);

            MethodInfo invokeMethod = typeof(IDelegationUnit).GetMethod(
                "Invoke",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new Type[] { typeof(Type), typeof(MethodInfo), typeof(Type[]), typeof(object), typeof(object[]) },
                null);

            //container fields
            FieldBuilder interfaceBaseTypeField = container.DefineField(
                InterfaceBaseTypeName,
                typeof(Type),
                FieldAttributes.Private | FieldAttributes.Static
                );
            FieldBuilder methodArrayField = container.DefineField(
                "_methodArray",
                typeof(MethodInfo[]),
                FieldAttributes.Private | FieldAttributes.Static
                );
            //delegation unit fields
            FieldBuilder delegationUnitField = delegationUnitBuilder.DefineField(
                "_delegationUnit",
                typeof(IDelegationUnit),
                FieldAttributes.Private | FieldAttributes.InitOnly
                );
            FieldBuilder interfaceTypeField = delegationUnitBuilder.DefineField(
                "_interfaceType",
                typeof(Type),
                FieldAttributes.Private | FieldAttributes.InitOnly
                );
            FieldBuilder stateField = delegationUnitBuilder.DefineField(
                "_state",
                typeof(object),
                FieldAttributes.Private | FieldAttributes.InitOnly
                );

            //define constructor on interface
            generator = DefineConstructor(delegationUnitBuilder, baseTypeConstructor, delegationUnitField, interfaceTypeField, stateField);

            using (IEnumerator<MemberInfo> membersenumerator = members.GetEnumerator())
            {
                bool moved;
                MethodInfo methodInstance;
                MethodBuilder methodBuilder;
                string memberName;
                Type paramType;

                //define all methods (the list of members starts with methods)
                while ((moved = membersenumerator.MoveNext()) && membersenumerator.Current is MethodInfo)
                {
                    ParameterInfo[] parameters;
                    GenericTypeParameterBuilder[] argsBuilders;
                    methodInstance = (MethodInfo)membersenumerator.Current;
                    parameters = methodInstance.GetParameters();
                    memberName = GetMemberName(methodInstance);

                    methodBuilder = delegationUnitBuilder.DefineMethod(memberName,
                        MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                        CallingConventions.HasThis,
                        methodInstance.ReturnType,
                        Array.ConvertAll(parameters, (pi) => pi.ParameterType));

                    if (methodInstance.IsGenericMethod)
                    {
                        //adapt generic arguments from original method, if defined
                        Type[] genericArgs = methodInstance.GetGenericArguments();

                        argsBuilders = methodBuilder.DefineGenericParameters(Array.ConvertAll(genericArgs, (tpi) => tpi.Name));
                        //adapt type constraints for the argument
                        for (int i = 0; i < genericArgs.Length; i++)
                        {
                            argsBuilders[i].SetBaseTypeConstraint(genericArgs[i].BaseType);
                            argsBuilders[i].SetInterfaceConstraints(Array.FindAll(genericArgs, (ga) => ga.IsInterface));
                        }
                    }
                    else
                        argsBuilders = Array.Empty<GenericTypeParameterBuilder>();

                    delegationUnitBuilder.DefineMethodOverride(methodBuilder, methodInstance);

                    //define method parameters
                    int index = 1;
                    foreach (ParameterInfo pi in parameters)
                    {
                        ParameterBuilder pb = methodBuilder.DefineParameter(index, pi.Attributes, pi.Name);
                        if (pi.IsOptional)
                            pb.SetConstant(pi.DefaultValue);
                        index++;
                    }

                    {
                        generator = methodBuilder.GetILGenerator();
                        //initialize array that contains all parameters with which the method was invoked
                        //add type-Array containing generic arguments
                        //and submit them to the delegation unit set in the constructor

                        LocalBuilder paramArray = generator.DeclareLocal(typeof(object[]));
                        LocalBuilder genericArgs = generator.DeclareLocal(typeof(Type[]));

                        //initialize parameter array
                        generator.Emit(OpCodes.Ldc_I4, parameters.Length);
                        generator.Emit(OpCodes.Newarr, typeof(object));
                        generator.Emit(OpCodes.Stloc_0);

                        //initialize generic parameters array
                        generator.Emit(OpCodes.Ldc_I4, argsBuilders.Length);
                        generator.Emit(OpCodes.Newarr, typeof(Type));
                        generator.Emit(OpCodes.Stloc_1);

                        if (methodInstance.IsGenericMethod)
                        {
                            for (int i = 0, len = parameters.Length; i < len; i++)
                            {
                                //get type of generic argument i
                                generator.Emit(OpCodes.Ldloc_1);
                                generator.Emit(OpCodes.Ldc_I4, i);
                                generator.Emit(OpCodes.Ldtoken, argsBuilders[i]);
                                generator.Emit(OpCodes.Call, typeFromHandleMethod);
                                generator.Emit(OpCodes.Stelem_Ref);
                            }
                        }

                        for (int i = 0, len = parameters.Length; i < len; i++)
                        {
                            paramType = parameters[i].ParameterType;
                            generator.Emit(OpCodes.Ldloc_0);
                            generator.Emit(OpCodes.Ldc_I4, i);
                            generator.Emit(OpCodes.Ldarg, i + 1);
                            if (paramType.IsByRef)
                            {
                                paramType = paramType.GetElementType();
                                generator.Emit(OpCodes.Ldobj, paramType);
                            }
                            generator.Emit(OpCodes.Box, paramType);
                            generator.Emit(OpCodes.Stelem_Ref);
                        }

                        //load parameters and call _delegationUnit.InvokeInvoke(Type, System.Reflection.MethodInfo, Type[], object, object[])
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldfld, delegationUnitField); //load delgation unit

                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldfld, interfaceTypeField); //load interface type

                        generator.Emit(OpCodes.Ldsfld, methodArrayField);
                        generator.Emit(OpCodes.Ldc_I4, methods.Count);
                        generator.Emit(OpCodes.Ldelem_Ref); //load called interface-method

                        generator.Emit(OpCodes.Ldloc_1); //load generic parameters array

                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldfld, stateField); //load state

                        generator.Emit(OpCodes.Ldloc_0); //load parameter-array-reference

                        generator.Emit(OpCodes.Callvirt, invokeMethod);

                        //store ref params
                        for (int i = 0, len = parameters.Length; i < len; i++)
                        {
                            paramType = parameters[i].ParameterType;
                            if (paramType.IsByRef)
                            {
                                paramType = paramType.GetElementType();
                                generator.Emit(OpCodes.Ldarg, i + 1);
                                generator.Emit(OpCodes.Ldloc_0);
                                generator.Emit(OpCodes.Ldc_I4, i);
                                generator.Emit(OpCodes.Ldelem_Ref, i);
                                generator.Emit(OpCodes.Unbox_Any, paramType);
                                generator.Emit(OpCodes.Stobj, paramType); //, parameters[i].ParameterType
                            }
                        }
                        if (methodInstance.ReturnType != typeof(void))
                            generator.Emit(OpCodes.Unbox_Any, methodInstance.ReturnType);
                        else
                            generator.Emit(OpCodes.Pop);
                        generator.Emit(OpCodes.Ret);
                    }

                    methods.Add(methodInstance, methodBuilder);
                }

                if (moved) //define members other than methods
                    do
                    {
                        //set accessors from dictionary, if set on interface declaration
                        if (membersenumerator.Current is PropertyInfo propertyInstance)
                        {
                            ParameterInfo[] parameters = propertyInstance.GetIndexParameters();
                            MethodInfo getMethod = null, setMethod = null;
                            PropertyBuilder propertyBuilder = delegationUnitBuilder.DefineProperty(
                                propertyInstance.Name,
                                PropertyAttributes.None,
                                propertyInstance.PropertyType,
                                Array.ConvertAll(parameters, (p) => p.ParameterType)
                                );
                            if (propertyInstance.CanRead)
                            {
                                getMethod = propertyInstance.GetGetMethod(true);
                                propertyBuilder.SetGetMethod(methods[getMethod]);

                            }
                            if (propertyInstance.CanWrite)
                            {
                                setMethod = propertyInstance.GetSetMethod(true);
                                propertyBuilder.SetSetMethod(methods[setMethod]);
                            }

                            foreach (MethodInfo accessor in propertyInstance.GetAccessors(true))
                                if (accessor != getMethod && accessor != setMethod)
                                    propertyBuilder.AddOtherMethod(methods[accessor]);
                        }
                        else if (membersenumerator.Current is EventInfo eventInstance)
                        {
                            MethodInfo addMethod = null, removeMethod = null, raiseMethod = null;
                            EventBuilder eventBuilder = delegationUnitBuilder.DefineEvent(
                                GetMemberName(eventInstance),
                                EventAttributes.None,
                                eventInstance.EventHandlerType);

                            addMethod = eventInstance.GetAddMethod(true);
                            removeMethod = eventInstance.GetRemoveMethod(true);
                            raiseMethod = eventInstance.GetRaiseMethod(true);

                            if (addMethod != null)
                                eventBuilder.SetAddOnMethod(methods[addMethod]);
                            if (removeMethod != null)
                                eventBuilder.SetAddOnMethod(methods[removeMethod]);
                            if (raiseMethod != null)
                                eventBuilder.SetAddOnMethod(methods[raiseMethod]);

                            foreach (MethodInfo accessor in eventInstance.GetOtherMethods(true))
                                eventBuilder.AddOtherMethod(methods[accessor]);
                        }
                        else //check for unsupported members (Should not occur in this version as unsupported members were already filtered)
                            throw new NotSupportedException("Member not supported.");
                    } while (membersenumerator.MoveNext());
            }
            containerType = container.CreateTypeInfo();
            delegateType = delegationUnitBuilder.CreateTypeInfo();

            methodArray = new MethodInfo[methods.Count];
            methods.Keys.CopyTo(methodArray, 0);
            object[] param = new object[] { methodArray };

            containerType.InvokeMember("_methodArray", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField, null, null, param);
            param[0] = interfaceType;
            containerType.InvokeMember(InterfaceBaseTypeName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField, null, null, param);

            return delegateType;
        }

        private static ILGenerator DefineConstructor(TypeBuilder delegationUnitBuilder, ConstructorInfo baseTypeConstructor, FieldBuilder delegationUnitField, FieldBuilder interfaceTypeField, FieldBuilder stateField)
        {
            ILGenerator generator;
            ConstructorBuilder ctrb = delegationUnitBuilder.DefineConstructor(
                    MethodAttributes.Public | MethodAttributes.HideBySig,
                    CallingConventions.HasThis,
                    new Type[] { typeof(IDelegationUnit), typeof(Type), typeof(object) }
                    );
            generator = ctrb.GetILGenerator();
            //call constructor of base type
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, baseTypeConstructor);

            //store parameters delegationUnit, interfaceType and state in variables
            //interface type may differ from interface base type
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, delegationUnitField);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Stfld, interfaceTypeField);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_3);
            generator.Emit(OpCodes.Stfld, stateField);

            generator.Emit(OpCodes.Ret);
            return generator;
        }

        private static string GetMemberName(MemberInfo member)
        {
            if (member.DeclaringType.IsInterface)
                return member.DeclaringType.FullName + Type.Delimiter.ToString() + member.Name;
            else
                return member.Name;
        }

        private static TypeBuilder DeclareType(TypeBuilder parent, string typeName, Type interfaceType, Type baseType)
        {
            TypeBuilder typeBuilder = parent.DefineNestedType(typeName,
                    TypeAttributes.Class | TypeAttributes.NestedPublic | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoClass,
                    baseType,
                    new Type[] { interfaceType });
            //add support for generic types
            if (interfaceType.IsGenericType)
            {
                Type[] genericParams = interfaceType.GetGenericArguments();
                GenericTypeParameterBuilder[] gtpb = typeBuilder.DefineGenericParameters(Array.ConvertAll(genericParams, (p) => p.Name));

                //set generic parameters using interface or type constraints
                for (int i = 0; i < gtpb.Length; i++)
                {
                    gtpb[i].SetBaseTypeConstraint(genericParams[i].BaseType);
                    gtpb[i].SetInterfaceConstraints(Array.FindAll(genericParams[i].GetGenericParameterConstraints(), (tp) => tp.IsInterface));
                }
            }

            return typeBuilder;
        }

        //TODO: document marshalByRef
        /// <summary>
        /// Creates an interface delegate defined by the generic argument.
        /// </summary>
        /// <typeparam name="TInterface">The interface delegate to create a delegate for.</typeparam>
        /// <param name="unit">The delegation unit that all calls are delegated to.</param>
        /// <returns>The interface delegate that was created.</returns>
        public static TInterface CreateInstance<TInterface>(bool marshalByRef, IDelegationUnit unit)
        {
            return (TInterface)CreateInstance(marshalByRef, typeof(TInterface), unit, null);
        }

        /// <summary>
        /// Creates an interface delegate defined by the generic argument.
        /// </summary>
        /// <typeparam name="TInterface">The interface delegate to create a delegate for.</typeparam>
        /// <param name="state">An additional state object that can be used for the identification of the type delegate.
        /// This value defaults to null, if omitted.</param>
        /// <param name="unit">The delegation unit that all calls are delegated to.</param>
        /// <returns>The interface delegate that was created.</returns>
        public static TInterface CreateInstance<TInterface>(bool marshalByRef, IDelegationUnit unit, object state)
        {
            return (TInterface)CreateInstance(marshalByRef, typeof(TInterface), unit, state);
        }

        /// <summary>
        /// Creates an interface delegate for the specified interface type.
        /// </summary>
        /// <param name="interfaceType">The interface type to create a delegate for.</typeparam>
        /// <param name="unit">The delegation unit that all calls are delegated to.</param>
        /// <returns>The interface delegate that was created.</returns>
        public static object CreateInstance(bool marshalByRef, Type interfaceType, IDelegationUnit unit)
        {
            return CreateInstance(marshalByRef, interfaceType, unit, null);
        }

        /// <summary>
        /// Creates an interface delegate for the specified interface type.
        /// </summary>
        /// <param name="interfaceType">The interface type to create a delegate for.</typeparam>
        /// <param name="state">An additional state object that can be used for the identification of the type delegate.
        /// This value defaults to null, if omitted.</param>
        /// <param name="unit">The delegation unit that all calls are delegated to.</param>
        /// <returns>The interface delegate that was created.</returns>
        public static object CreateInstance(bool marshalByRef, Type interfaceType, IDelegationUnit unit, object state)
        {
            return Activator.CreateInstance(GetInterfaceDelegateType(interfaceType, marshalByRef), unit, interfaceType, state);
        }

        private static ICollection<MemberInfo> GetMembers(Type type)
        {
            LinkedList<MemberInfo> members = new LinkedList<MemberInfo>();
            GetMembersHelper(type, members, new LinkedList<Type>());
            return members;
        }

        private static void GetMembersHelper(Type type, LinkedList<MemberInfo> members, ICollection<Type> addedTypes)
        {
            //check, whether an interface implementation has already been added (two interfaces may inherit the same interface)
            if (!addedTypes.Contains(type))
            {
                //declare methods before other supported members
                foreach (MemberInfo mi in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    if (mi is MethodInfo)
                        members.AddFirst(mi);
                    else if (mi is PropertyInfo || mi is EventInfo)
                        members.AddLast(mi);
                    else
                        throw new NotSupportedException("Interface member " + mi.Name + " is not supported.");
                foreach (Type ifc in type.GetInterfaces())
                    GetMembersHelper(ifc, members, addedTypes);
            }
        }

        private sealed class KeyType { }
    }
}
