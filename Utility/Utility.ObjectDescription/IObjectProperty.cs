using System;

namespace Utility.ObjectDescription
{
    public interface IObjectProperty<T>
    {
        T Value { get; set; }
        bool IsReadonly { get; }
    }

    public interface IObjectProperty
    {
        object Value { get; set; }
        bool IsReadonly { get; }

        Type PropertyType { get; }
    }
}