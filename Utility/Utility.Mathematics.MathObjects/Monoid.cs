using System;
using Utility;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public class Monoid<T> : SetBinaryOperation<T>
    {
        public T NeutralElement { get; }

        public Monoid(BinaryOperation<T> operation, T neutralElement)
            : base(operation)
        {
            NeutralElement = neutralElement;
        }
    }
}