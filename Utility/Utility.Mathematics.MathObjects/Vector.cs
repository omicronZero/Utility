using System;
using System.Collections.Generic;
using System.Text;
using Utility.Contexts;

namespace Utility.Mathematics.MathObjects
{
    //TODO: make serializable
    public struct Vector<TVector, TScalar>
    {
        public VectorSpace<TVector, TScalar> VectorSpace { get; }

        public TVector Value { get; }

        public Vector(VectorSpace<TVector, TScalar> vectorSpace, TVector value)
        {
            if (vectorSpace == null)
                throw new ArgumentNullException(nameof(vectorSpace));

            VectorSpace = vectorSpace;
            Value = value;
        }

        private Vector(TVector value)
        {
            VectorSpace = null;
            Value = value;
        }

        public VectorSpace<TVector, TScalar> ContextualVectorSpace
        {
            get => VectorSpace ?? ThreadContextual<VectorSpace<TVector, TScalar>>.CurrentContext;
        }

        public static Vector<TVector, TScalar> operator +(Vector<TVector, TScalar> left, TVector right)
        {
            return new Vector<TVector, TScalar>(left.VectorSpace, left.ContextualVectorSpace.Add(left.Value, right));
        }

        public static Vector<TVector, TScalar> operator +(TVector left, Vector<TVector, TScalar> right)
        {
            return new Vector<TVector, TScalar>(right.VectorSpace, right.ContextualVectorSpace.Add(left, right.Value));
        }

        public static Vector<TVector, TScalar> operator -(Vector<TVector, TScalar> left, TVector right)
        {
            return new Vector<TVector, TScalar>(left.VectorSpace, left.ContextualVectorSpace.Subtract(left.Value, right));
        }

        public static Vector<TVector, TScalar> operator -(TVector left, Vector<TVector, TScalar> right)
        {
            return new Vector<TVector, TScalar>(right.VectorSpace, right.ContextualVectorSpace.Subtract(left, right.Value));
        }

        public static Vector<TVector, TScalar> operator -(Vector<TVector, TScalar> value)
        {
            return new Vector<TVector, TScalar>(value.VectorSpace, value.ContextualVectorSpace.Multiply(value.VectorSpace.MinusOne, value.Value));
        }

        public static Vector<TVector, TScalar> operator +(Vector<TVector, TScalar> value)
        {
            //assuming that e * value = value
            return value;
        }

        public static Vector<TVector, TScalar> operator *(TScalar left, Vector<TVector, TScalar> right)
        {
            return new Vector<TVector, TScalar>(right.VectorSpace, right.ContextualVectorSpace.Multiply(left, right.Value));
        }

        public static Vector<TVector, TScalar> operator *(Vector<TVector, TScalar> left, TScalar right)
        {
            return new Vector<TVector, TScalar>(left.VectorSpace, left.ContextualVectorSpace.Multiply(left.Value, right));
        }

        public static Vector<TVector, TScalar> operator /(Vector<TVector, TScalar> left, TScalar right)
        {
            return new Vector<TVector, TScalar>(left.VectorSpace, left.ContextualVectorSpace.Divide(left.Value, right));
        }

        public static Vector<TVector, TScalar> CreateContextDependent(TVector vector)
        {
            return new Vector<TVector, TScalar>(vector);
        }

        public static Vector<TVector, TScalar> CreateOnCurrentContext(TVector vector)
        {
            return new Vector<TVector, TScalar>(ThreadContextual<VectorSpace<TVector, TScalar>>.CurrentContext, vector);
        }

        public static Vector<TVector, TScalar> CreateOnDefaultContext(TVector vector)
        {
            return new Vector<TVector, TScalar>(GlobalContext<VectorSpace<TVector, TScalar>>.GetDefaultContext(), vector);
        }
    }
}
