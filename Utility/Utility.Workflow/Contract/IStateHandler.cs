using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Workflow.Contract
{
    /// <summary>
    /// Provides external mechanisms to save and load states. See remarks for an example scenario.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the state handler manages.</typeparam>
    /// <typeparam name="TState">The type of state managed by the state handler. The state contains all relevant recovery and state information.</typeparam>
    /// <remarks>
    /// A state handler may be frequently useful when actions are required to be definitely executed safely.
    /// For example, when accessing and writing to a file a sudden application crash may leave the file in a
    /// corrupted state. Here is where a state handler can come in: If a state handler is implemented with
    /// <typeparamref name="TEntity"/> being set to some file-referencing type, it can persist the state
    /// (e.g., a backup and the actions to be performed may be persisted) to handle such a crash. Furthermore,
    /// recovery procedures may be implemented.
    /// 
    /// While many such procedures require specific implementations in the state handler, an abstract approach
    /// is provided here.
    /// 
    /// Furthermore, note that the state handler must check for external corruption (e.g., files changed by the user, ...) on its own.
    /// </remarks>
    public interface IStateHandler<TEntity, TState>
    {
        TState SaveState(TEntity entity);

    }
}
