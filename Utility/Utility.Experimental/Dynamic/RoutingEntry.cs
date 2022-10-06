using System;
using System.Collections.Generic;

namespace Utility.Dynamic
{
    public struct RoutingEntry<TDescription, TState> : IEquatable<RoutingEntry<TDescription, TState>>
    {
        public int Generation { get; }
        public int Index { get; }
        public int FrameIndex { get; }
        public TDescription Description { get; }
        public TState State { get; }

        public RoutingEntry(int generation, int index, int frameIndex, TDescription description, TState state)
        {
            Generation = generation;
            Index = index;
            FrameIndex = frameIndex;
            Description = description;
            State = state;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Generation ^ Index ^ FrameIndex ^ (State?.GetHashCode() ?? 0) ^ (Description?.GetHashCode() ?? 0);
        }

        public bool Equals(RoutingEntry<TDescription, TState> other)
        {
            return Generation == other.Generation
                && Index == other.Index
                && FrameIndex == other.FrameIndex
                && EqualityComparer<TDescription>.Default.Equals(Description, other.Description)
                && EqualityComparer<TState>.Default.Equals(State, other.State);
        }

        public static bool operator ==(RoutingEntry<TDescription, TState> left, RoutingEntry<TDescription, TState> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RoutingEntry<TDescription, TState> left, RoutingEntry<TDescription, TState> right)
        {
            return !left.Equals(right);
        }
    }
}