using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Workflow.Collections.Labeled
{
    internal interface ILabeled
    {
        void UpdateLabel(Label label, int newIndex);
        int Count { get; }
    }
}
