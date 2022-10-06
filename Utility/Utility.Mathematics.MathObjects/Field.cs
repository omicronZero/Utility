using System;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public class Field<T>
    {
        public CommutativeGroup<T> Addition { get; }
        public CommutativeGroup<T> Multiplication { get; }

        public Field(CommutativeGroup<T> addition, CommutativeGroup<T> multiplication)
        {
            if (addition == null)
                throw new ArgumentNullException(nameof(addition));
            if (multiplication == null)
                throw new ArgumentNullException(nameof(multiplication));

            Addition = addition;
            Multiplication = multiplication;
        }

        public T One => Multiplication.NeutralElement;
        public T Zero => Addition.NeutralElement;

        public T MinusOne => Addition.InverseOperation[One];

        public T Negate(T value) => Addition.InverseOperation[value];
        public T MultiplicativeInverse(T value) => Multiplication.InverseOperation[value];
    }
}