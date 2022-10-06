using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public interface ICastable
    {
        bool Is<T>();
        bool Is<T>(out T value);
        T As<T>() where T : class;

        T Cast<T>();

        bool HasInstanceType { get; }
        Type InstanceType { get; }
        object GetInstance();
    }
}
