using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSpecTests.Mocks
{
    public class SerializableComplex
    {
        public SerializableComplex Other { get; }
        public SerializableSimple Value { get; }

        public SerializableComplex(int value)
        {
            Value = new SerializableSimple(value);
        }

        public SerializableComplex(SerializableComplex other, int value)
        {
            Other = other;
            Value = new SerializableSimple(value);
        }
    }
}
