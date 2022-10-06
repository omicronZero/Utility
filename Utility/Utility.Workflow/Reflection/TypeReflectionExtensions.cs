using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utility.Reflection
{
    //named this way as TypeExtensions is already occupied by System.Reflection.TypeExtensions
    public static class TypeReflectionExtensions
    {
        private const string OpExplicitName = "op_Explicit";
        private const string OpImplicitName = "op_Implicit";

        public static MethodInfo[] GetOperators(this Type type, Operators operatorKind)
        {
            return (MethodInfo[])type.GetMember(OperatorHelper.GetOperatorName(operatorKind), MemberTypes.Method, BindingFlags.Public | BindingFlags.Static);
        }

        public static MethodInfo GetBinaryOperator(BinaryOperator binaryOperator, Type leftOperandType, Type rightOperandType)
        {
            return GetBinaryOperatorCore(OperatorHelper.GetOperatorName(binaryOperator), leftOperandType, rightOperandType);
        }

        public static MethodInfo GetBinaryOperator(BinaryOperator binaryOperator, Type leftOperandType, Type rightOperandType, Type returnType)
        {
            MethodInfo m = GetBinaryOperator(binaryOperator, leftOperandType, rightOperandType);

            if (returnType != null && !returnType.IsAssignableFrom(m.ReturnType))
                m = null;

            return m;
        }

        public static MethodInfo GetUnaryOperator(UnaryOperator unaryOperator, Type operandType)
        {
            return GetUnaryOperatorCore(OperatorHelper.GetOperatorName(unaryOperator), operandType);
        }

        public static MethodInfo GetUnaryOperator(UnaryOperator unaryOperator, Type operandType, Type returnType)
        {
            MethodInfo m = GetUnaryOperator(unaryOperator, operandType);

            if (returnType != null && !returnType.IsAssignableFrom(m.ReturnType))
                m = null;

            return m;
        }

        private static MethodInfo GetBinaryOperatorCore(string operatorName, Type leftOperandType, Type rightOperandType)
        {
            if (operatorName == null)
                throw new ArgumentNullException(nameof(operatorName));
            if (leftOperandType == null)
                throw new ArgumentNullException(nameof(leftOperandType));
            if (rightOperandType == null)
                throw new ArgumentNullException(nameof(rightOperandType));

            MethodInfo op = leftOperandType.GetMethod(operatorName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new Type[] { leftOperandType, rightOperandType }, null);

            if (op == null && leftOperandType != rightOperandType)
            {
                op = rightOperandType.GetMethod(operatorName,
                   BindingFlags.Public | BindingFlags.Static,
                   null,
                   new Type[] { leftOperandType, rightOperandType }, null);
            }

            return op;
        }

        private static MethodInfo GetUnaryOperatorCore(string operatorName, Type operandType)
        {
            if (operatorName == null)
                throw new ArgumentNullException(nameof(operatorName));
            if (operandType == null)
                throw new ArgumentNullException(nameof(operandType));

            MethodInfo op = operandType.GetMethod(operatorName,
                 BindingFlags.Public | BindingFlags.Static,
                null,
                new Type[] { operandType }, null);

            return op;
        }

        public static Func<TSource, TTarget> GetConvert<TSource, TTarget>(bool allowExplicit)
        {
            var toOp = GetOperatorConvertTo<TSource, TTarget>(allowExplicit);
            var fromOp = GetOperatorConvertFrom<TSource, TTarget>(allowExplicit);

            if (toOp != null && fromOp != null)
                throw new ArgumentException("Both indicated types offer implementations for conversion operations between the two types.");

            return toOp ?? fromOp;
        }

        public static Func<TSource, TTarget> GetOperatorConvertFrom<TSource, TTarget>(bool allowExplicit)
        {
            return (allowExplicit || !CastOp<TSource, TTarget>.IsConvertToExplicit) ? CastOp<TSource, TTarget>.ConvertTo : null;
        }

        public static Func<TSource, TTarget> GetOperatorConvertTo<TSource, TTarget>(bool allowExplicit)
        {
            return (allowExplicit || !CastOp<TSource, TTarget>.IsConvertFromExplicit) ? CastOp<TSource, TTarget>.ConvertFrom : null;
        }

        public static Type GetCommonBaseType(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            using var enr = types.GetEnumerator();

            if (!enr.MoveNext())
            {
                return null;
            }

            Type baseType = enr.Current;

            if (baseType == null)
                throw new ArgumentException("The supplied types must not be null.", nameof(types));

            if (baseType.IsInterface)
                throw new ArgumentException("Interface types not allowed.", nameof(types));

            while (enr.MoveNext())
            {
                Type ctp = enr.Current;

                if (ctp == null)
                    throw new ArgumentException("The supplied types must not be null.", nameof(types));

                if (ctp.IsInterface)
                    throw new ArgumentException("Interface types not allowed.", nameof(types));

                if (ctp.IsAssignableFrom(baseType))
                {
                    baseType = ctp;
                }
                else if (!baseType.IsAssignableFrom(ctp))
                {
                    do
                    {
                        baseType = baseType.BaseType;
                    } while (!baseType.IsAssignableFrom(ctp));
                }
            }

            return baseType;
        }

        public static IEnumerable<Type> GetCommonInterfaces(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            using var enr = types.GetEnumerator();

            if (!enr.MoveNext())
                return Array.Empty<Type>();

            var interfaces = new List<Type>(enr.Current.GetInterfaces());

            foreach (Type tp in types)
            {
                if (tp == null)
                    throw new ArgumentException("The supplied types must not be null.", nameof(types));

                for (int i = interfaces.Count - 1; i >= 0; i--)
                {
                    if (!interfaces[i].IsAssignableFrom(tp))
                        interfaces.RemoveAt(i);
                }
            }

            return interfaces;
        }

        //TODO: add more conversion methods

        public static IEnumerable<MethodInfo> GetOperatorExplicits(this Type type, bool includeInheritedOperators = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetMember(OpExplicitName, MemberTypes.Method, BindingFlags.Public | BindingFlags.Static | (includeInheritedOperators ? BindingFlags.FlattenHierarchy : 0)).OfType<MethodInfo>();
        }

        public static IEnumerable<MethodInfo> GetOperatorImplicits(this Type type, bool includeInheritedOperators = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetMember(OpImplicitName, MemberTypes.Method, BindingFlags.Public | BindingFlags.Static | (includeInheritedOperators ? BindingFlags.FlattenHierarchy : 0)).OfType<MethodInfo>();
        }

        public static IEnumerable<MethodInfo> GetConvertOperator(this Type type, bool includeExplicit, bool includeImplicit, bool includeInheritedOperators = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            IEnumerable<MethodInfo> enumerable = null;

            if (includeImplicit)
                enumerable = GetOperatorImplicits(type, includeInheritedOperators);

            if (includeExplicit)
            {
                var t = GetOperatorExplicits(type, includeInheritedOperators);

                if (enumerable == null)
                    enumerable = t;
                else
                    enumerable = enumerable.Concat(t);
            }

            return enumerable ?? Array.Empty<MethodInfo>();
        }

        /// <summary>
        /// Gets an enumeration of operators that allow converting instances of type <paramref name="sourceType"/> to <paramref name="targetType"/>.
        /// See <see cref="GetOperatorConvertFrom(Type, Type, bool, bool, bool, bool)"/> for the differences between it and <see cref="GetOperatorConvertTo(Type, Type, bool, bool, bool, bool)"/>.
        /// </summary>
        /// <param name="sourceType">The type from which the returned method should convert.</param>
        /// <param name="targetType">The type to which the returned method should convert.</param>
        /// <param name="includeExplicit">True, if explicit conversion operators are to be included.</param>
        /// <param name="includeImplicit">True, if implicit conversion operators are to be included-</param>
        /// <param name="exactMatch">True, if operand types are to match exactly despite type compatibility, otherwise false.</param>
        /// <param name="includeInheritedOperators">True, if matching operators defined on the source and target base types should be included in the enumeration.</param>
        /// <param name="includeTargetOperators">True, if not only operators on the source type should be included but also the ones defined on the target type.</param>
        /// <returns>An enumeration of conversion operators. This often has only zero or one member.</returns>
        public static IEnumerable<MethodInfo> GetOperatorConvertTo(this Type sourceType, Type targetType, bool includeExplicit, bool includeImplicit = true, bool exactMatch = false, bool includeInheritedOperators = false, bool includeTargetOperators = true)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            return GetOperatorConvertToCore(sourceType, targetType, exactMatch, includeExplicit, includeImplicit, true, includeTargetOperators, includeInheritedOperators);
        }

        /// <summary>
        /// Gets an enumeration of operators that allow converting instances of type <paramref name="sourceType"/> to <paramref name="targetType"/>. See remarks for further information.
        /// </summary>
        /// <param name="targetType">The type to which the returned method should convert.</param>
        /// <param name="sourceType">The type from which the returned method should convert.</param>
        /// <param name="includeExplicit">True, if explicit conversion operators are to be included.</param>
        /// <param name="includeImplicit">True, if implicit conversion operators are to be included-</param>
        /// <param name="exactMatch">True, if operand types are to match exactly despite type compatibility, otherwise false.</param>
        /// <param name="includeInheritedOperators">True, if matching operators defined on the source and target base types should be included in the enumeration.</param>
        /// <param name="includeTargetOperators">True, if not only operators on the source type should be included but also the ones defined on the target type.</param>
        /// <returns>An enumeration of conversion operators. This often has only zero or one member.</returns>
        /// <remarks>
        /// The main difference to <see cref="GetOperatorConvertTo(Type, Type, bool, bool, bool, bool)"/> occurs if <paramref name="includeTargetOperators"/> is false. In this case, only methods are enumerated
        /// that are defined on the target type. Imagine, a conversion from type A to B is defined on type A. Querying <see cref="GetOperatorConvertTo(Type, Type, bool, bool, bool, bool)"/> will yield the conversion
        /// for both target inclusion modes. However, as the operation is defined on A, <see cref="GetOperatorConvertFrom(Type, Type, bool, bool, bool, bool)"/> will only return the operator if
        /// <paramref name="includeTargetOperators"/> is true as otherwise the source type's operators will not be enumerated.
        /// </remarks>
        public static IEnumerable<MethodInfo> GetOperatorConvertFrom(this Type targetType, Type sourceType, bool includeExplicit, bool includeImplicit = true, bool exactMatch = false, bool includeInheritedOperators = false, bool includeTargetOperators = true)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            return GetOperatorConvertToCore(sourceType, targetType, exactMatch, includeExplicit, includeImplicit, includeTargetOperators, true, includeInheritedOperators);
        }

        private static IEnumerable<MethodInfo> GetOperatorConvertToCore(Type sourceType, Type targetType, bool exactMatch, bool includeExplicit, bool includeImplicit, bool includeSourceOperators, bool includeTargetOperators, bool includeInheritedOperators)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            var ops = includeSourceOperators ? GetConvertOperator(sourceType, includeExplicit, includeImplicit, includeInheritedOperators) : null;

            if (includeTargetOperators)
            {
                ops = ops.ConcatOrInvariant(EnumerableExtensions.Lazy(() => GetConvertOperator(targetType, includeExplicit, includeImplicit, includeInheritedOperators)));
            }

            if (ops == null)
                yield break;

            foreach (MethodInfo m in ops)
            {
                if (exactMatch ? (m.ReturnType != targetType) : !targetType.IsAssignableFrom(m.ReturnType))
                    continue;

                ParameterInfo[] p = m.GetParameters();

                if (p.Length != 1)
                    continue;

                var param = p[0];

                if (exactMatch ? (param.ParameterType == sourceType) : param.ParameterType.IsAssignableFrom(sourceType))
                    yield return m;
            }
        }

        public static IEnumerable<Type> GetGenericImplementations(Type type, Type baseType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (baseType == null)
                throw new ArgumentNullException(nameof(baseType));

            if (!baseType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Generic type definition expected for base type.", nameof(baseType));
            }

            if (baseType.IsInterface)
            {
                foreach (Type tp in type.GetInterfaces())
                {
                    if (tp.IsGenericType && tp.GetGenericTypeDefinition() == baseType)
                    {
                        yield return tp;
                    }
                }
            }
            else
            {
                for (; type != null; type = type.BaseType)
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                    {
                        yield return type;
                        yield break;
                    }
                }
            }
        }

        private static class CastOp<TSource, TTarget>
        {
            public static Func<TSource, TTarget> ConvertTo { get; private set; }
            public static Func<TSource, TTarget> ConvertFrom { get; private set; }
            public static bool IsConvertToExplicit { get; private set; }
            public static bool IsConvertFromExplicit { get; private set; }

            static CastOp()
            {
                IsConvertFromExplicit = false;
                IsConvertToExplicit = false;

                ConvertTo = null;
                ConvertFrom = null;

                foreach (var implicitOp in GetOperatorConvertTo(typeof(TSource), typeof(TTarget), includeExplicit: false, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false))
                {
                    ConvertTo = (Func<TSource, TTarget>)implicitOp.CreateDelegate(typeof(Func<TSource, TTarget>));
                }

                if (ConvertTo == null)
                    foreach (var explicitOp in GetOperatorConvertTo(typeof(TSource), typeof(TTarget), includeExplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false))
                    {
                        ConvertTo = (Func<TSource, TTarget>)explicitOp.CreateDelegate(typeof(Func<TSource, TTarget>));
                        IsConvertToExplicit = true;
                    }

                foreach (var implicitOp in GetOperatorConvertFrom(typeof(TSource), typeof(TTarget), includeExplicit: false, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false))
                {
                    ConvertFrom = (Func<TSource, TTarget>)implicitOp.CreateDelegate(typeof(Func<TSource, TTarget>));
                }

                if (ConvertTo == null)
                    foreach (var explicitOp in GetOperatorConvertFrom(typeof(TSource), typeof(TTarget), includeExplicit: true, exactMatch: true, includeInheritedOperators: false, includeTargetOperators: false))
                    {
                        ConvertFrom = (Func<TSource, TTarget>)explicitOp.CreateDelegate(typeof(Func<TSource, TTarget>));
                        IsConvertToExplicit = true;
                    }
            }
        }
    }
}
