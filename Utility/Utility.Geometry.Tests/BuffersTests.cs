using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Utility.Geometry.Buffers;
using Utility;
using System.Linq;
using Utility.Mathematics;

namespace Utility.Geometry.Buffers
{
    public class BuffersTests
    {
        [Fact]
        public void TestAccessViewAccess()
        {
            //configuration

            const int w = 100;
            Random random = new Random(0);
            const int turns = 1000;

            Interval<long> xRange = new Interval<long>(0, w);
            Interval<long> yRange = new Interval<long>(0, w);

            long x = 0, y = 0;

            Func<long, long, float> eval = (dx, dy) => (dx + dy) / (float)(w * w);
            Action<long, long, float> actor = (dx, dy, v) =>
            {
                if (dx != x || dy != y)
                    throw new ArgumentException("AccessView: Assignment failed.");

                if (v != eval(x, y))
                    throw new ArgumentException("AccessView: Assigned value did not pass.");
            };

            //arrange

            AccessView2D<float> readAccessor = new AccessView2D<float>(eval, actor);
            AccessRangeView2D<float> rangedReadAccessor = new AccessRangeView2D<float>(readAccessor, xRange, yRange);

            //act

            //assert

            (long, long)[] randoms = Enumerable.Range(0, turns)
                .Select((s) => (3 * random.NextDouble() - 1, 3 * random.NextDouble() - 1)) //generates (rx, ry) as elements of [-1, 2)^2 that represents [0, 1) with a margin of 1 around it
                .Select((arg) => ((long)Math.Round(arg.Item1 * w), (long)Math.Round(arg.Item2 * w))).ToArray(); //scale to (-w, ..., 2w - 1)^2

            Assert.All(randoms, arg =>
                 {
                     (x, y) = arg;

                     if (readAccessor[x, y] != eval(x, y))
                         throw new ArgumentException("AccessView: Evaluation failed.");

                     readAccessor[x, y] = eval(x, y);
                 });

            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessor[-1, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessor[-1, -1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessor[1, -1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessor[w, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessor[w, w]);
            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessor[w, w - 1]);

            _ = rangedReadAccessor[0, 0];
            _ = rangedReadAccessor[w - 1, 0];
            _ = rangedReadAccessor[0, w - 1];
            _ = rangedReadAccessor[w - 1, w - 1];
        }

        [Fact]
        public void TestAccessViewAccessGeneric()
        {
            //configuration

            const int w = 100;
            Random random = new Random(0);
            const int turns = 1000;

            Interval<long> xRange = new Interval<long>(0, w);
            Interval<long> yRange = new Interval<long>(0, w);

            long x = 0, y = 0;

            Func<long, long, float> eval = (dx, dy) => (dx + dy) / (float)(w * w);
            Action<long, long, float> actor = (dx, dy, v) =>
            {
                if (dx != x || dy != y)
                    throw new ArgumentException("AccessView: Assignment failed.");

                if (v != eval(x, y))
                    throw new ArgumentException("AccessView: Assigned value did not pass.");
            };

            //arrange

            AccessView2D<float, long> readAccessorGeneric = new AccessView2D<float>(eval, actor);
            AccessRangeView2D<float, long> rangedReadAccessorGeneric = new AccessRangeView2D<float, long>(readAccessorGeneric, xRange, yRange);

            //act

            //assert

            (long, long)[] randoms = Enumerable.Range(0, turns)
                .Select((s) => (3 * random.NextDouble() - 1, 3 * random.NextDouble() - 1)) //generates (rx, ry) as elements of [-1, 2)^2 that represents [0, 1) with a margin of 1 around it
                .Select((arg) => ((long)Math.Round(arg.Item1 * w), (long)Math.Round(arg.Item2 * w))).ToArray(); //scale to (-w, ..., 2w - 1)^2

            Assert.All(randoms, arg =>
            {
                (x, y) = arg;

                if (readAccessorGeneric[x, y] != eval(x, y))
                    throw new ArgumentException("AccessView: Evaluation failed.");

                readAccessorGeneric[x, y] = eval(x, y);
                readAccessorGeneric[x, y] = eval(x, y);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessorGeneric[-1, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessorGeneric[-1, -1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessorGeneric[1, -1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessorGeneric[w, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessorGeneric[w, w]);
            Assert.Throws<ArgumentOutOfRangeException>(() => rangedReadAccessorGeneric[w, w - 1]);

            _ = rangedReadAccessorGeneric[0, 0];
            _ = rangedReadAccessorGeneric[w - 1, 0];
            _ = rangedReadAccessorGeneric[0, w - 1];
            _ = rangedReadAccessorGeneric[w - 1, w - 1];
        }

        [Fact]
        public void TestAccessViewProperties()
        {
            AccessView2D<string> readOnly = new AccessView2D<string>((x, y) => x.ToString() + y, null);
            AccessView2D<string> writeOnly = new AccessView2D<string>(null, (x, y, v) => { });
            AccessView2D<string> none = new AccessView2D<string>(null, null);

            Assert.True(readOnly.CanRead);
            Assert.False(readOnly.CanWrite);
            Assert.True(readOnly.IsReadOnly);
            Assert.False(readOnly.IsWriteOnly);


            Assert.True(writeOnly.CanWrite);
            Assert.False(writeOnly.CanRead);
            Assert.True(writeOnly.IsWriteOnly);
            Assert.False(writeOnly.IsReadOnly);


            Assert.False(none.CanWrite);
            Assert.False(none.CanRead);
            Assert.False(none.IsWriteOnly);
            Assert.False(none.IsReadOnly);
        }

        [Fact]
        public void TestAccessViewPropertiesGeneric()
        {
            AccessView2D<string, long> readOnly = new AccessView2D<string>((x, y) => x.ToString() + y, null);
            AccessView2D<string, long> writeOnly = new AccessView2D<string>(null, (x, y, v) => { });
            AccessView2D<string, long> none = new AccessView2D<string>(null, null);

            Assert.True(readOnly.CanRead);
            Assert.False(readOnly.CanWrite);
            Assert.True(readOnly.IsReadOnly);
            Assert.False(readOnly.IsWriteOnly);


            Assert.True(writeOnly.CanWrite);
            Assert.False(writeOnly.CanRead);
            Assert.True(writeOnly.IsWriteOnly);
            Assert.False(writeOnly.IsReadOnly);


            Assert.False(none.CanWrite);
            Assert.False(none.CanRead);
            Assert.False(none.IsWriteOnly);
            Assert.False(none.IsReadOnly);
        }

        [Fact]
        public void TestArrayAccess()
        {
            const int width = 100, height = 100;

            int[] array = new int[width * height];

            Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayAccess2D<int>(array, -1, height));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayAccess2D<int>(array, width, -1));
            Assert.Throws<ArgumentException>(() => new ArrayAccess2D<int>(array, width + 1, height));

            ArrayAccess2D<int> arrayAccess = new ArrayAccess2D<int>(array, width, height);

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[-1, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[-1, -1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[width, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[width, height]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[width + 1, height + 1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[width * height]);

            _ = arrayAccess[width - 1, 0];
            _ = arrayAccess[width - 1, height - 1];
            _ = arrayAccess[0, 0];

            Assert.True(arrayAccess.Length == array.Length);
        }

        [Fact]
        public void TestArrayAccessOffset()
        {
            const int width = 100, height = 100;
            const int offset = 5;

            int[] array = new int[width * height + offset];

            Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayAccess2D<int>(array, -1, width, height));
            Assert.Throws<ArgumentException>(() => new ArrayAccess2D<int>(array, offset + 1, width, height));

            ArrayAccess2D<int> arrayAccess = new ArrayAccess2D<int>(array, offset, width, height);

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[-1, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[-1, -1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[width, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[width, height]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[width + 1, height + 1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = arrayAccess[width * height]);

            _ = arrayAccess[width - 1, 0];
            _ = arrayAccess[width - 1, height - 1];
            _ = arrayAccess[0, 0];

            Assert.True(arrayAccess.Length == array.Length - offset);
            Assert.True(arrayAccess.Length == arrayAccess.LongLength);
            Assert.True(arrayAccess.Width == width);
            Assert.True(arrayAccess.Height == height);
        }

        [Fact]
        public void TestBuffer2DFunctionality()
        {
            int w = 10;
            int h = 10;
            Func<long, long, long> eval = (x, y) => x * y;
            long[,] array = new long[w, h];
            var intervalX = new Interval<long>(0, w);
            var intervalY = new Interval<long>(0, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                    array[x, y] = eval(x, y);
            }

            Buffer2D<long> buffer = new Buffer2DMock<long>(new AccessRangeView2D<long>(eval, null, intervalX, intervalY));
            ArrayBuffer2D<long> arrayBuffer = buffer.AsArrayBuffer();


            Assert.All(Enumerable.Range(0, h).SelectMany((y) => Enumerable.Range(0, w).Select((x) => (x, y))), (c) =>
                {
                    var (x, y) = c;

                    if (array[x, y] != buffer[x, y])
                    {
                        throw new ArgumentException("Buffer error.");
                    }

                    if (array[x, y] != arrayBuffer[x, y])
                    {
                        throw new ArgumentException("Array buffer error.");
                    }
                });
        }

        [Fact]
        public void TestBuffer2DProperties()
        {
            Buffer2D<int> buffer = new ArrayBuffer2D<int>(1, 1);
            IBuffer<int> ibuffer = buffer;

            Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayBuffer2D<int>(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayBuffer2D<int>(1, -1));

            new ArrayBuffer2D<int>(0, 0);

            Assert.Equal(1, buffer.Width);
            Assert.Equal(1, buffer.Height);
            Assert.Equal(2, ibuffer.Dimensions);
            Assert.False(ibuffer.IsReadOnly);

            Assert.All(Enumerable.Range(0, ibuffer.Dimensions), (i) =>
            {
                Assert.Equal(1, ibuffer.GetLength(i));
            });
        }

        [Fact]
        public void TestBuffer3DProperties()
        {
            Buffer3D<int> buffer = new ArrayBuffer3D<int>(1, 1, 1);
            IBuffer<int> ibuffer = buffer;

            Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayBuffer3D<int>(-1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayBuffer3D<int>(1, -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayBuffer3D<int>(1, 1, -1));

            new ArrayBuffer3D<int>(0, 0, 0);

            Assert.Equal(1, buffer.Width);
            Assert.Equal(1, buffer.Height);
            Assert.Equal(1, buffer.Depth);
            Assert.Equal(3, ibuffer.Dimensions);
            Assert.False(ibuffer.IsReadOnly);

            Assert.All(Enumerable.Range(0, ibuffer.Dimensions), (i) =>
            {
                Assert.Equal(1, ibuffer.GetLength(i));
            });
        }

        [Fact]
        public void TestBuffer3DFunctionality()
        {
            int w = 10;
            int h = 10;
            int d = 10;

            Func<long, long, long, long> eval = (x, y, z) => x * y * z;
            long[,,] array = new long[w, h, d];

            var intervalX = new Interval<long>(0, w);
            var intervalY = new Interval<long>(0, h);
            var intervalZ = new Interval<long>(0, d);

            for (int z = 0; z < d; z++)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                        array[x, y, z] = eval(x, y, z);
                }
            }

            Buffer3D<long> buffer = new Buffer3DMock<long>(new AccessRangeView3D<long>(eval, null, intervalX, intervalY, intervalZ));
            ArrayBuffer3D<long> arrayBuffer = buffer.AsArrayBuffer();

            Assert.All(Enumerable.Range(0, w * h * d).Select((i) => (i % h, (i / h) % d, i / (h * d))), (c) =>
            {
                var (x, y, z) = c;

                if (array[x, y, z] != buffer[x, y, z])
                {
                    throw new ArgumentException("Buffer error.");
                }

                if (array[x, y, z] != arrayBuffer[x, y, z])
                {
                    throw new ArgumentException("Array buffer error.");
                }
            });
        }

        [Fact]
        public void TestTexture2D()
        {
            Func<double, double, double> eval = (x, y) => Math.Max(x * (1 - y), y * (1 - x));

            Texture2D<double> texture = Texture2D<double>.FromDelegate(eval);

            Random rnd = new Random();

            Assert.All(Enumerable.Range(0, 1000), (i) =>
            {
                var (dx, dy) = (rnd.NextDouble(), rnd.NextDouble());

                Assert.Equal(texture[dx, dy], eval(dx, dy));
            });
        }

        [Fact]
        public void TestTransformedTexture2D()
        {
            Func<double, double, Vector2r> eval = (x, y) => (x, y);

            TransformedTexture2D<Vector2r> texture = TransformedTexture2D<Vector2r>.TransformTexture(Texture2D<Vector2r>.FromDelegate(eval));
            var transform = new Matrix2x3r(0.5, 0, 0,
                                           0, 0.5, 0);

            texture.Transform = transform;

            Assert.Equal(texture.Transform, transform);

            Random rnd = new Random();

            Assert.All(Enumerable.Range(0, 1000), (i) =>
            {
                var (dx, dy) = (rnd.NextDouble(), rnd.NextDouble());

                var (cx, cy, cz) = Matrix2x3r.Transform((dx, dy), transform);

                Assert.Equal(texture[dx, dy], eval(cx, cy));
            });
        }

        [Fact]
        public void TestTexture3D()
        {
            Func<double, double, double, double> eval = (x, y, z) => Math.Max(x * (1 - y), y * (1 - x));

            Texture3D<double> texture = Texture3D<double>.FromDelegate(eval);

            Random rnd = new Random();

            Assert.All(Enumerable.Range(0, 1000), (i) =>
            {
                var (dx, dy, dz) = (rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());

                Assert.Equal(texture[dx, dy, dz], eval(dx, dy, dz));
            });
        }

        [Fact]
        public void TestTransformedTexture3D()
        {
            Func<double, double, double, Vector3r> eval = (x, y, z) => (x, y, z);

            TransformedTexture3D<Vector3r> texture = TransformedTexture3D<Vector3r>.TransformTexture(Texture3D<Vector3r>.FromDelegate(eval));

            var transform = new Matrix3x4r(0.5, 0, 0, 0,
                                           0, 0.5, 0, 0,
                                           0, 0, 0.5, 0);

            texture.Transform = transform;

            Assert.Equal(texture.Transform, transform);

            Random rnd = new Random();

            Assert.All(Enumerable.Range(0, 1000), (i) =>
            {
                var (dx, dy, dz) = (rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());

                var (cx, cy, cz, dw) = Matrix3x4r.Transform((dx, dy, dz), transform);

                Assert.Equal(texture[dx, dy, dz], eval(cx, cy, cz));
            });
        }

        [Fact]
        public void TestWrapModes()
        {
            Assert.Equal(0, TextureWrap.Clamp(-1));
            Assert.Equal(0, TextureWrap.Clamp(-0.5));
            Assert.Equal(1, TextureWrap.Clamp(2));
            Assert.Equal(0.5, TextureWrap.Clamp(0.5));
            Assert.Equal(0.25, TextureWrap.Clamp(0.25));

            Assert.Equal(1, TextureWrap.Wrap(-TextureWrap.AboveOne));
            Assert.Equal(0, TextureWrap.Wrap(-1));
            Assert.Equal(0, TextureWrap.Wrap(2 * TextureWrap.AboveOne));
            Assert.Equal(1, TextureWrap.Wrap(1));
            Assert.Equal(0.25, TextureWrap.Wrap(0.25));
            
            Assert.Equal(1, TextureWrap.WrapMirrored(-1));
            Assert.Equal(0, TextureWrap.WrapMirrored(2));
            Assert.Equal(1, TextureWrap.WrapMirrored(1));
            Assert.Equal(0.25, TextureWrap.WrapMirrored(0.25));
            Assert.Equal(0.75, TextureWrap.WrapMirrored(1.25));
            Assert.Equal(0.75, TextureWrap.WrapMirrored(-0.75));
            Assert.Equal(0.75, TextureWrap.WrapMirrored(-1.25));
            Assert.Equal(0.25, TextureWrap.WrapMirrored(-1.75));
        }

        [Fact]
        public void TestWrappedTexture()
        {
            Texture2D<Vector2r> texture2 = Texture2D<Vector2r>.FromDelegate((x, y) => (x, y));
            Texture3D<Vector3r> texture3 = Texture3D<Vector3r>.FromDelegate((x, y, z) => (x, y, z));

            Random rnd = new Random();

            double xxxx = 1;
            double dv = 1;

            while (xxxx + dv / 2 != 1)
            {
                dv /= 2;
            }

            WrapMode[] wrapModes = new WrapMode[] { WrapMode.Clamp, WrapMode.None, WrapMode.Wrap, WrapMode.WrapMirrored };

            foreach (WrapMode wrapZ in wrapModes)
            {
                Func<double, double> zwrap = TextureWrap.GetWrapFunction(wrapZ);

                foreach (WrapMode wrapY in wrapModes)
                {
                    Func<double, double> ywrap = TextureWrap.GetWrapFunction(wrapY);

                    foreach (WrapMode wrapX in wrapModes)
                    {
                        Func<double, double> xwrap = TextureWrap.GetWrapFunction(wrapX);

                        var tex2 = Texture2D<Vector2r>.Wrap(texture2, wrapX, wrapY);
                        var tex3 = Texture3D<Vector3r>.Wrap(texture3, wrapX, wrapY, wrapZ);

                        for (double z = -5; z < 5; z += 0.25)
                        {
                            for (double y = -5; y < 5; y += 0.25)
                            {
                                for (double x = -5; x < 5; x += 0.25)
                                {
                                    Assert.Equal((xwrap(x), ywrap(y)), tex2[x, y]);
                                    Assert.Equal((xwrap(x), ywrap(y), zwrap(z)), tex3[x, y, z]);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
