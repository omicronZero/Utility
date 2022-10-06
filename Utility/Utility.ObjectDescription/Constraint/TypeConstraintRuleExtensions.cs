using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.ObjectDescription.Constraint
{
    public static class TypeConstraintRuleExtensions
    {
        public static ObjectConstraintBuilder OfType<T>(this ObjectConstraintBuilder builder)
        {
            builder.Append(TypeConstraintRule<T>.Instance);

            return builder;
        }

        public static ObjectConstraintBuilder OfType(this ObjectConstraintBuilder builder, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            builder.Append(TypeConstraintRule.Get(type));

            return builder;
        }

        public static Type GetTypeConstraint(this IEnumerable<ObjectConstraint> instances)
        {
            return GetTypeConstraint(instances, hasConstraint: out _);
        }

        public static Type GetTypeConstraint(this IEnumerable<ObjectConstraint> instances, out Type[] interfaceTypes)
        {
            return GetTypeConstraint(instances, out _, out interfaceTypes);
        }

        public static Type GetTypeConstraint(this IEnumerable<ObjectConstraint> instances, out bool hasConstraint)
        {
            return GetTypeConstraint(instances, false, out hasConstraint, out _);
        }

        public static Type GetTypeConstraint(this IEnumerable<ObjectConstraint> instances, out bool hasConstraint, out Type[] interfaceTypes)
        {
            return GetTypeConstraint(instances, true, out hasConstraint, out interfaceTypes);
        }


        public static Type GetTypeConstraint(IEnumerable<ObjectConstraint> instances, bool interfaces, out bool hasConstraint, out Type[] interfaceTypes)
        {
            if (instances == null)
                throw new ArgumentNullException(nameof(instances));

            using (var instenr = instances.GetEnumerator())
            {
                if (!instenr.MoveNext())
                {
                    interfaceTypes = null;
                    hasConstraint = false;
                    return typeof(object);
                }

                var ifcs = interfaces ? new HashSet<Type>() : null;

                hasConstraint = false;

                Type baseType = null;

                do
                {
                    ObjectConstraint instance = instenr.Current;

                    using (var enr = instance.GetConstraints<TypeConstraintRule>().GetEnumerator())
                    {
                        if (!enr.MoveNext())
                            continue;

                        if (!hasConstraint)
                        {
                            hasConstraint = true;
                            baseType = enr.Current.Type;
                        }

                        while (enr.MoveNext())
                        {
                            Type cbt = enr.Current.Type;

                            if (baseType != null && baseType != cbt)
                            {
                                //narrow type down
                                if (baseType.IsAssignableFrom(cbt))
                                {
                                    baseType = cbt;
                                }
                                else
                                {
                                    baseType = null;
                                }
                            }

                            if (interfaces)
                            {
                                foreach (Type ifc in cbt.GetInterfaces())
                                {
                                    ifcs.Add(ifc);
                                }
                            }
                        }

                    }
                } while (instenr.MoveNext());

                interfaceTypes = ifcs?.ToArray();
                return hasConstraint ? baseType : typeof(object);
            }
        }

        public static Type GetTypeConstraint(this ObjectConstraint instance)
        {
            return GetTypeConstraint(instance, hasConstraint: out _);
        }

        public static Type GetTypeConstraint(this ObjectConstraint instance, out Type[] interfaceTypes)
        {
            return GetTypeConstraint(instance, out _, out interfaceTypes);
        }

        public static Type GetTypeConstraint(this ObjectConstraint instance, out bool hasConstraint)
        {
            return GetTypeConstraint(instance, false, out hasConstraint, out _);
        }

        public static Type GetTypeConstraint(this ObjectConstraint instance, out bool hasConstraint, out Type[] interfaceTypes)
        {
            return GetTypeConstraint(instance, true, out hasConstraint, out interfaceTypes);
        }

        private static Type GetTypeConstraint(this ObjectConstraint instance, bool interfaces, out bool hasConstraint, out Type[] interfaceTypes)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            using (var enr = instance.GetConstraints<TypeConstraintRule>().GetEnumerator())
            {
                if (!enr.MoveNext())
                {
                    interfaceTypes = null;
                    hasConstraint = false;
                    return typeof(object);
                }

                hasConstraint = true;

                var ifcs = interfaces ? new HashSet<Type>(enr.Current.Type.GetInterfaces()) : null;
                Type baseType = enr.Current.Type;

                while (enr.MoveNext())
                {
                    Type cbt = enr.Current.Type;

                    if (baseType != null && baseType != cbt)
                    {
                        //narrow type down
                        if (baseType.IsAssignableFrom(cbt))
                        {
                            baseType = cbt;
                        }
                        else
                        {
                            baseType = null;
                        }
                    }

                    if (interfaces)
                    {
                        foreach (Type ifc in cbt.GetInterfaces())
                        {
                            ifcs.Add(ifc);
                        }
                    }
                }

                interfaceTypes = ifcs?.ToArray();
                return baseType;
            }
        }
    }
}
