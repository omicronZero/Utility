using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public abstract class VectorSpace<TVector, TScalar>
    {
        public CommutativeGroup<TVector> Addition { get; }
        public Field<TScalar> ScalarField { get; }
        public BinaryOperation<TScalar, TVector, TVector> LeftMultiplication { get; }
        public BinaryOperation<TVector, TScalar, TVector> RightMultiplication { get; }

        public VectorSpace(
                CommutativeGroup<TVector> addition,
                Field<TScalar> scalarField,
                BinaryOperation<TScalar, TVector, TVector> leftMultiplication,
                BinaryOperation<TVector, TScalar, TVector> rightMultiplication
            )
        {
            if (addition == null)
                throw new ArgumentNullException(nameof(addition));

            if (scalarField == null)
                throw new ArgumentNullException(nameof(scalarField));

            if (leftMultiplication == null)
                throw new ArgumentNullException(nameof(leftMultiplication));

            if (rightMultiplication == null)
                throw new ArgumentNullException(nameof(rightMultiplication));

            Addition = addition;
            ScalarField = scalarField;
            LeftMultiplication = leftMultiplication;
            RightMultiplication = rightMultiplication;
        }

        public TScalar MinusOne => ScalarField.MinusOne;

        public TScalar Multiply(TScalar scalar1, TScalar scalar2)
        {
            return ScalarField.Multiplication[scalar1, scalar2];
        }

        public TScalar Add(TScalar scalar1, TScalar scalar2)
        {
            return ScalarField.Addition[scalar1, scalar2];
        }

        public TVector Multiply(TScalar scalar, TVector vector)
        {
            return LeftMultiplication[scalar, vector];
        }

        public TVector Multiply(TVector vector, TScalar scalar)
        {
            return RightMultiplication[vector, scalar];
        }

        public TVector Divide(TVector vector, TScalar scalar)
        {
            return RightMultiplication[vector, ScalarField.MultiplicativeInverse(scalar)];
        }

        public TVector Add(TVector vector1, TVector vector2)
        {
            return Addition[vector1, vector2];
        }

        public TVector Negate(TVector vector)
        {
            return Multiply(MinusOne, vector);
        }

        public TScalar Negate(TScalar scalar)
        {
            return ScalarField.Negate(scalar);
        }

        public TVector Subtract(TVector vector1, TVector vector2)
        {
            return Addition[vector1, Negate(vector2)];
        }
    }
}
