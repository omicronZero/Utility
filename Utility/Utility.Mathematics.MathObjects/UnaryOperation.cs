using System;
using Utility.ObjectDescription.Constraint;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public class UnaryOperation<T, TResult> : Operation<Func<T, TResult>>
    {
        public UnaryOperation(Func<T, TResult> function, ObjectConstraint returnConstraint, ObjectConstraint parameterConstraint)
            : base(function, returnConstraint, parameterConstraint)
        { }
        public TResult this[T value] => Function(value);
    }

    public class UnaryOperation<T> : UnaryOperation<T, T>
    {
        public UnaryOperation(Func<T, T> function, ObjectConstraint returnConstraint, ObjectConstraint parameterConstraint)
            : base(function, returnConstraint, parameterConstraint)
        { }
    }
}