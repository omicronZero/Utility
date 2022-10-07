using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Workflow.Services
{
    public interface IServiceRegistry : IServiceProvider
    {
        void RegisterService(Type serviceType, object instance);
        bool RegisterIfNotAvailable(Type serviceType, Func<object> serviceFactory, out object service);
    }
}
