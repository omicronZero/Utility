using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Workflow.Contract
{
    public interface IContractAction
    {
        bool TryUndo(IContractContext context);

        bool TryDo(IContractContext context);
    }
}
