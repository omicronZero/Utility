using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility
{
    public partial struct RangeDecimal : ISerializable
    {
        private RangeDecimal(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            Start = info.GetDecimal("Start");
            End = info.GetDecimal("End");

        }

        private void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Start", Start);
            info.AddValue("End", End);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }
    }
}
