using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Dynamic
{
    public interface IComputationFlowHandler<TCall, THandler, TDescription, TState>
    {
        void Enqueue(TCall callHandler, RoutingSources<TState, TDescription> routingSources);
        THandler Build();
    }
}
