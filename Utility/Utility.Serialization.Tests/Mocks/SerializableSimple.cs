using Utility.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSpecTests.Mocks
{
    public class SerializableSimple : IObjectSerializable
    {
        public int Value { get; }

        public SerializableSimple(int value)
        {
            Value = value;
        }

        protected SerializableSimple(IObjectReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Value = reader.Read<int>();
        }

        public void GetObjectData(IObjectWriter writer)
        {
            writer.Write(Value);
        }
    }
}
