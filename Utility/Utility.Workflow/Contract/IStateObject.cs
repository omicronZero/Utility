using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Workflow.Contract
{
    public interface IStateObject
    {
        object SaveState();
        void LoadState(object state);
    }

    public interface IStateObject<T> : IStateObject
    {
        new T SaveState();

        void LoadState(T state);

        void IStateObject.LoadState(object state)
        {
            if (!(state is T v))
                throw new ArgumentException($"Expected state of type {typeof(T).FullName}.");

            LoadState(v);
        }
    }
}
