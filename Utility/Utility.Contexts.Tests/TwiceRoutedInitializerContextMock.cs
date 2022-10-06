using Utility.Contexts;

namespace Utility.Contexts.Tests
{
    [PropagateContextInitializer(typeof(First))]
    public class TwiceRoutedInitializerContextMock
    {
        [PropagateContextInitializer(typeof(Second))]
        public class First : TwiceRoutedInitializerContextMock
        {
        }

        [DefaultContextInitializer()]
        public class Second : First
        {
        }
    }
}
