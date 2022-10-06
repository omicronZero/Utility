using System;
using Utility.Contexts;

namespace Utility.Contexts.Tests
{
    public class InitializableContextMockInitializer : ContextInitializer
    {
        protected override object CreateDefaultObjectCore(Type type)
        {
            if (type != typeof(InitializableContextMock))
                throw new ArgumentException("The specified type is not supported.", nameof(type));

            return new InitializableContextMock(1);
        }
    }
}
