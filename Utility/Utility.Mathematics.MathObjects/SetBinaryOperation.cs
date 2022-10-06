using System;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public class SetBinaryOperation<T>
    {
        public BinaryOperation<T> Operation { get; }

        public SetBinaryOperation(BinaryOperation<T> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            Operation = operation;
        }

        public T this[T left, T right] => Operation.Function(left, right);
    }
}