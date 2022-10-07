using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utility.Workflow.Serialization
{
    public interface IObjectSerializer
    {
        void Serialize<T>(Stream stream, T instance, IServiceProvider context);
        T Deserialize<T>(Stream stream, IServiceProvider context);
    }
}
