using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public class Group<T> : Monoid<T>
    {
        public UnaryOperation<T> InverseOperation { get; }

        public Group(BinaryOperation<T> operation, T neutralElement, UnaryOperation<T> inverseOperation)
            : base(operation, neutralElement)
        {
            if (inverseOperation == null)
                throw new ArgumentNullException(nameof(inverseOperation));

            InverseOperation = inverseOperation;
        }
    }
}
