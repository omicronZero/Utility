using System;
using System.Collections.Generic;
using System.Text;
using Utility.Mathematics;

namespace Utility.Geometry.Buffers
{
    public abstract class TransformedTexture3D<T> : Texture3D<T>
    {
        private static readonly Matrix3x4r IdentityMatrix = new Matrix3x4r(1, 0, 0, 0,
                                                                           0, 1, 0, 0,
                                                                           0, 0, 1, 0);

        public Matrix3x4r Transform { get; set; }

        protected abstract T Get(double transformedX, double transformedY, double transformedZ);

        protected TransformedTexture3D(Matrix3x4r transform)
        {
            Transform = transform;
        }

        protected TransformedTexture3D()
            : this(IdentityMatrix)
        { }

        public override T this[double x, double y, double z]
        {
            get
            {
                Vector3r transformed = Vector4r.Transform(Transform, new Vector4r(x, y, z, 1));
                return Get(transformed.X, transformed.Y, transformed.Z);
            }
        }

        public static TransformedTexture3D<T> TransformTexture(Texture3D<T> underlyingTexture)
        {
            return TransformTexture(underlyingTexture, IdentityMatrix);
        }

        public static TransformedTexture3D<T> TransformTexture(Texture3D<T> underlyingTexture, Matrix3x4r transform)
        {
            if (underlyingTexture == null)
                throw new ArgumentNullException(nameof(underlyingTexture));

            return new TransformedTexture(underlyingTexture, transform);
        }

        private sealed class TransformedTexture : TransformedTexture3D<T>
        {
            private readonly Texture3D<T> _underlyingTexture;

            public TransformedTexture(Texture3D<T> underlyingTexture)
                : this(underlyingTexture, IdentityMatrix)
            { }

            public TransformedTexture(Texture3D<T> underlyingTexture, Matrix3x4r transform)
                : base(transform)
            {
                if (underlyingTexture == null)
                    throw new ArgumentNullException(nameof(underlyingTexture));

                _underlyingTexture = underlyingTexture;
            }

            protected override T Get(double transformedX, double transformedY, double transformedZ)
            {
                return _underlyingTexture[transformedX, transformedY, transformedZ];
            }
        }
    }
}
