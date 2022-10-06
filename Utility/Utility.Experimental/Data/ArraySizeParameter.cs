using System;

namespace Utility.Data
{
    [Serializable]
    public struct ArraySizeParameter
    {
        public long[] ArraySizes { get; set; }

        public ArraySizeParameter(long arraySize)
            : this(new long[] { 1 })
        { }

        public ArraySizeParameter(long[] arraySizes)
        {
            if (arraySizes == null)
                throw new ArgumentNullException(nameof(arraySizes));

            ArraySizes = arraySizes;
        }
    }

    [Serializable]
    public struct ArraySizeParameter<TInParameters>
    {
        public long[] ArraySizes { get; set; }

        public TInParameters InParameters { get; set; }

        public ArraySizeParameter(long arraySize, TInParameters inParameters)
            : this(new long[] { 1 }, inParameters)
        { }

        public ArraySizeParameter(long[] arraySizes, TInParameters inParameters)
        {
            if (arraySizes == null)
                throw new ArgumentNullException(nameof(arraySizes));

            ArraySizes = arraySizes;
            InParameters = inParameters;
        }
    }
}