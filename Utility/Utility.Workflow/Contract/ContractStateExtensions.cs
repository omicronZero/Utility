using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Workflow.Contract
{
    public static class ContractStateExtensions
    {
        public static void Do(this IContractAction action, IContractContext context)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (!action.TryDo(context))
                throw new BadContractActionException("The action has already been performed.");
        }

        public static void Undo(this IContractAction action, IContractContext context)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (!action.TryUndo(context))
                throw new BadContractActionException("The action has not been performed.");
        }
    }
}
