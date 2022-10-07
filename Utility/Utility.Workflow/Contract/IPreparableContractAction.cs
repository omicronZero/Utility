using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Workflow.Contract
{
    public interface IPreparableContractAction : IContractAction
    {
        /// <summary>
        /// Prepares the current instance.
        /// </summary>
        /// <returns></returns>
        bool Prepare(IContractContext context);
    }
}
