using Utility.Contexts;

namespace Utility.Contexts.Tests
{
    [ContextInitializer(typeof(InitializableContextMockInitializer))]
    public class InitializableContextMock
    {
        public int Value { get; }

        public InitializableContextMock(int value)
        {
            Value = value;
        }
    }
}
