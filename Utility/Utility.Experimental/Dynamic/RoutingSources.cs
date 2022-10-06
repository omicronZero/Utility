using System;
using System.Collections.Generic;

namespace Utility.Dynamic
{
    public class RoutingSources<TDescription, TState>
    {
        private readonly List<List<RoutingEntry<TDescription, TState>>> _entriesList;

        public IList<RoutingEntry<TDescription, TState>> Entries { get; }

        internal RoutingSources()
        {
            _entriesList = new List<List<RoutingEntry<TDescription, TState>>>();
            Entries = new Collections.CompositeList<RoutingEntry<TDescription, TState>>(_entriesList);
        }

        public int FrameCount => _entriesList.Count;

        public Collections.ReadOnlyList<RoutingEntry<TDescription, TState>> GetEntries(int index)
        {
            return new Collections.ReadOnlyList<RoutingEntry<TDescription, TState>>(_entriesList[index]);
        }

        internal void AddFrame(List<RoutingEntry<TDescription, TState>> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            _entriesList.Add(entries);
        }
    }
}