using Utility.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSpecTests.Mocks
{
    [ObjectSerializer(typeof(SerializableCustom.Serializer))]
    public sealed class SerializableCustom
    {
        public int Value { get; }

        public SerializableCustom(int value)
        {
            Value = value;
        }

        private sealed class Serializer : StrictObjectSerializer<SerializableCustom>
        {
            public override SerializableCustom GetObject(IObjectReader source)
            {
                int value = source.Read<int>();

                return new SerializableCustom(value);
            }

            protected override void GetObjectDataCore(IObjectWriter target, SerializableCustom instance)
            {
                target.Write(instance.Value);
            }
        }
    }
}
