using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public class CommutativeGroup<T> : Group<T>
    {
        public CommutativeGroup(BinaryOperation<T> operation, T neutralElement, UnaryOperation<T> inverseOperation) : base(operation, neutralElement, inverseOperation)
        { }
    }
}
