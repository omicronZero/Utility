using Utility.Serialization;

namespace DataSpecTests.Mocks
{
    public class DummyReadWriter : IObjectReader, IObjectWriter
    {
        public static DummyReadWriter Instance { get; } = new DummyReadWriter();

        public T Read<T>()
        {
            return default;
        }

        public void Write<T>(T instance)
        { }
    }
}
