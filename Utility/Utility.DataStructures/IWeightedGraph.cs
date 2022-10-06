using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public interface IWeightedGraph<TNode, TWeight>
    {
        ICollection<TNode> Nodes { get; }
        IDictionary<Edge<TNode>, TWeight> EdgeMap { get; }

        bool IsDirected { get; }
        bool WeightsReadonly { get; }

        IDictionary<TNode, TWeight> GetIns(TNode node);
        IDictionary<TNode, TWeight> GetOuts(TNode node);

        //TODO: add Copy-function
    }
}
