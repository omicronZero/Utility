using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility
{
    public partial struct RangeDateTime : ISerializable
    {
        private RangeDateTime(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            Start = info.GetDateTime("Start");
            End = info.GetDateTime("End");
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
