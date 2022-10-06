using System;

namespace Utility.Dynamic.Disassembly
{
    public struct ArrayDescription
    {
        public TypeDescription Type { get; }
        public ArrayShape ArrayShape { get; }

        public Type ToType()
        {
            for (int i = 0; i < ArrayShape.LowerBoundsCount; i++)
                if (ArrayShape.GetLowerBound(i) != 0)
                    throw new NotSupportedException("System.Type does not support resolving array types with lower boundaries differing from zero.");

            return Type.ToType().MakeArrayType(unchecked((int)ArrayShape.Rank));
        }

        public ArrayDescription(TypeDescription type, ArrayShape arrayShape)
        {
            Type = type;
            ArrayShape = arrayShape;
        }
    }
}