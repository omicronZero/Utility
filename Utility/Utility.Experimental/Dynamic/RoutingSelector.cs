using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Dynamic
{
    public abstract class RoutingSelector<TDescription, TState>
    {
        //targets: parameters, fields, etc. to which to store the content in
        //sources: parameters, fields, etc. from which to receive the content
        //  the source kind (parameters, fields, etc.) is switched to a compatible kind by the routing, if necessary
        public abstract void Route(RoutingTarget<TDescription, TState>[] targets, RoutingSources<TDescription, TState> routingSources);
    }
}
