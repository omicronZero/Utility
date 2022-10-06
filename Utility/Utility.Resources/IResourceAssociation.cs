using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data.Resources
{
    public interface IResourceAssociation<in TID, TAssociation>
    {
        TAssociation GetAssociation(TID id);
        void SetAssociation(TID id, TAssociation value);
        bool AssociationsFixed { get; }
    }
}
