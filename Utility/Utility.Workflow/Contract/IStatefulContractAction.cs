using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Workflow.Contract
{
    public interface IStatefulContractAction<TEntity, TState> : IContractAction
    {
        void SaveState(IStateHandler<TEntity, TState> stateHandler);
    }
}
