using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Utility.ObjectDescription;
using Utility;
using System.Linq;
using Utility.ObjectDescription.Constraint;
using Utility.Collections;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public abstract class Operation
    {
        public abstract int Arity { get; }
        public abstract ObjectConstraint ReturnConstraint { get; }
        public abstract ObjectConstraint GetPropertyConstraint(int index);

        public abstract object GetValue(params object[] parameters);

        public virtual bool ProvidesHandler => false;

        public virtual Delegate Handler
        {
            get { throw new NotSupportedException("Handler could not be provided by the current type."); }
        }

        public IList<ObjectConstraint> GetParameters()
        {
            return new DelegateList<ObjectConstraint>(GetPropertyConstraint, () => Arity, true);
        }
    }

    public abstract class Operation<TFunc> : Operation
        where TFunc : Delegate
    {
        private static readonly ObjectConstraint TypeReturnConstraint;
        private static readonly System.Collections.ObjectModel.ReadOnlyCollection<ObjectConstraint> TypeParameterConstraint;

        public override ObjectConstraint ReturnConstraint { get; }
        public System.Collections.ObjectModel.ReadOnlyCollection<ObjectConstraint> ParameterConstraints { get; }

        public TFunc Function { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline", Justification = "We always need all of them.")]
        static Operation()
        {
            MethodInfo invokeMethod = typeof(TFunc).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
            ParameterInfo[] param = invokeMethod.GetParameters();

            bool hasOutParams = false;

            foreach (var p in param)
            {
                if (p.IsOut)
                {
                    hasOutParams = true;
                    break;
                }
            }

            if (!hasOutParams)
            {
                TypeParameterConstraint = Array.AsReadOnly(Array.ConvertAll(param, (p) => TypeConstraintRule.CreateConstraint(p.ParameterType)));
                TypeReturnConstraint = TypeConstraintRule.CreateConstraint(invokeMethod.ReturnType);
            }
        }

        protected Operation(TFunc function, ObjectConstraint returnConstraint, params ObjectConstraint[] parameterConstraint)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            if (TypeParameterConstraint == null)
            {
                throw new ArgumentException("Functions with out-parameters are not supported by operations.", nameof(TFunc));
            }

            if (parameterConstraint != null)
            {
                if (TypeParameterConstraint.Count != parameterConstraint.Length)
                {
                    throw new ArgumentException("Parameter constraints must either be null or their length must match the arity of the supplied function.", nameof(parameterConstraint));
                }
            }

            if (returnConstraint != null)
            {
                if (!TypeReturnConstraint.IsValidConstraint(returnConstraint))
                    throw new ArgumentException("Return constraint is less restrictive than type's constraint.", nameof(returnConstraint));
            }
            else
                returnConstraint = TypeReturnConstraint;

            if (parameterConstraint != null)
            {
                if (!TypeReturnConstraint.IsValidConstraint(returnConstraint))
                    throw new ArgumentException("Return constraint is less restrictive than type's constraint.", nameof(returnConstraint));

                ParameterConstraints = Array.AsReadOnly(Enumerable.Range(0, parameterConstraint.Length).Select((i) => ParameterAnd(i, TypeParameterConstraint[i], parameterConstraint[i])).ToArray());
            }
            else
                ParameterConstraints = TypeParameterConstraint;


            ReturnConstraint = returnConstraint;


            Function = function;
        }

        public sealed override int Arity => TypeParameterConstraint.Count;

        public sealed override ObjectConstraint GetPropertyConstraint(int index)
        {
            Util.ValidateIndex(TypeParameterConstraint.Count, index);

            return TypeParameterConstraint[index];
        }

        public override object GetValue(params object[] parameters)
        {
            return Function.DynamicInvoke(parameters);
        }

        public override Delegate Handler => Function;

        public override bool ProvidesHandler => true;

        private static ObjectConstraint ReturnValueAnd(ObjectConstraint typeParameterConstraint, ObjectConstraint parameterConstraint)
        {
            ObjectConstraint c = ObjectConstraint.And(typeParameterConstraint, parameterConstraint);

            if (c.IsEmptySet)
                throw new ArgumentException($"The restrictions on the return value do not allow any value.");

            return c;
        }

        private static ObjectConstraint ParameterAnd(int parameterIndex, ObjectConstraint typeParameterConstraint, ObjectConstraint parameterConstraint)
        {
            ObjectConstraint c = ObjectConstraint.And(typeParameterConstraint, parameterConstraint);

            if (c.IsEmptySet)
                throw new ArgumentException($"The restrictions on parameter {parameterIndex} do not allow any value.");

            return c;
        }
    }
}
