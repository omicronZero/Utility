using Utility.Contexts;

namespace Utility.Contexts.Tests
{
    [PropagateContextInitializer(typeof(First))]
    public class RoutedInitializerContextMock
    {
        [DefaultContextInitializer()]
        public class First : RoutedInitializerContextMock
        {
        }
    }
}
