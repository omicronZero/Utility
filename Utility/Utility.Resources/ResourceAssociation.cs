using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Utility.Data.Resources
{
    public class ResourceAssociation<TID, TAssociation> : IResourceAssociation<TID, TAssociation>
    {
        private readonly ConcurrentDictionary<TID, TAssociation> _associations;
        private readonly Func<TID, TAssociation> _associationInitializer;
        private readonly Func<TAssociation, bool> _associationValidator;

        public bool AssociationsFixed { get; }

        public ResourceAssociation(
            Func<TID, TAssociation> associationInitializer,
            Func<TAssociation, bool> associationValidator,
            bool associationsFixed)
        {
            if (associationInitializer == null)
            {
                if (associationsFixed)
                    throw new ArgumentException("Assocation initializer must not be null if associations are fixed.");

                associationInitializer = (id) => default;
            }

            _associations = new ConcurrentDictionary<TID, TAssociation>();

            _associationInitializer = associationInitializer;
            _associationValidator = associationValidator;
            AssociationsFixed = associationsFixed;
        }

        public TAssociation this[TID id]
        {
            get => _associations.GetOrAdd(id, _associationInitializer);
            set
            {
                if (AssociationsFixed)
                    throw new NotSupportedException("Associations are fixed for this resource dictionary.");

                SetAssociationCore(id, value);
            }
        }

        protected virtual void SetAssociationCore(TID id, TAssociation value)
        {
            if (_associationValidator != null && !_associationValidator(value))
                throw new ArgumentException("The indicated association is invalid for the resource.", nameof(value));

            _associations[id] = value;
        }

        protected void ResetAssociations()
        {
            _associations.Clear();
        }

        protected void ResetAssocation(TID id)
        {
            _associations.TryRemove(id, out _);
        }

        TAssociation IResourceAssociation<TID, TAssociation>.GetAssociation(TID id)
        {
            return this[id];
        }

        void IResourceAssociation<TID, TAssociation>.SetAssociation(TID id, TAssociation value)
        {
            this[id] = value;
        }
    }
}
