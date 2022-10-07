using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Utility.Workflow.Contract
{
    public interface IContractContext
    {
        IServiceProvider ServiceProvider { get; }
    }
}
