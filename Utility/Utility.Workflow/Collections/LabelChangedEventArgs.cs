using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Collections
{
    public class LabelChangedEventArgs : EventArgs
    {
        public int OldIndex { get; }
        public int ChangedItemIndex { get; }

        public LabelChangedEventArgs(int oldIndex, int changedItemIndex)
        {
            OldIndex = oldIndex;
            ChangedItemIndex = changedItemIndex;
        }
    }
}
