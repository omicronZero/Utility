using System;
using Utility.ObjectDescription.Constraint;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public abstract class BinaryOperation<TLeft, TRight, TResult> : Operation<Func<TLeft, TRight, TResult>>
    {
        protected BinaryOperation(Func<TLeft, TRight, TResult> function, ObjectConstraint returnConstraint = null, ObjectConstraint leftParameterConstraint = null, ObjectConstraint rightParameterConstraint = null)
            : base(function, returnConstraint, leftParameterConstraint, rightParameterConstraint)
        { }

        public TResult this[TLeft left, TRight right] => Function(left, right);
    }

    public abstract class BinaryOperation<T> : BinaryOperation<T, T, T>
    {
        protected BinaryOperation(Func<T, T, T> function, ObjectConstraint returnConstraint = null, ObjectConstraint leftParameterConstraint = null, ObjectConstraint rightParameterConstraint = null)
            : base(function, returnConstraint, leftParameterConstraint, rightParameterConstraint)
        { }
    }
}
