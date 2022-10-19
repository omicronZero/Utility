using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Serialization.Serializers
{
    public class LeafWriter : IObjectWriter
    {
        private readonly IObjectWriter _innerWriter;

        private readonly ISerializerProvider _serializerProvider;

        public LeafWriter(IObjectWriter innerWriter, ISerializerProvider? serializerProvider = null)
        {
            _innerWriter =innerWriter ?? throw new ArgumentNullException(nameof(innerWriter));
            _serializerProvider = serializerProvider ?? Serialization.DefaultProvider;
        }

        public void Write<T>(T instance)
        {
            var serializer = _serializerProvider.GetSerializer<T>();

            if (serializer == null)
                _innerWriter.Write(instance);
            else
                serializer.GetObjectData(this, instance);
        }
    }
}
