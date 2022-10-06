using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utility
{
    [Serializable]
    public struct UncertainBoolean : IEquatable<UncertainBoolean>, ISerializable
    {
        public static readonly UncertainBoolean True = new UncertainBoolean(State.True);
        public static readonly UncertainBoolean False = new UncertainBoolean(State.False);
        public static readonly UncertainBoolean Uncertain = new UncertainBoolean(State.Uncertain);

        private State _state;

        private UncertainBoolean(State state)
        {
            _state = state;
        }

        private UncertainBoolean(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            _state = (State)info.GetInt32("State");
        }

        public bool IsTrueOrUncertain => _state >= 0;
        public bool IsFalseOrUncertain => _state <= 0;
        public bool IsTrue => _state > 0;
        public bool IsFalse => _state < 0;
        public bool IsUncertain => _state == 0;
        public bool IsKnown => _state != 0;

        private int IntState => Math.Sign((int)_state);

        public override string ToString()
        {
            return IsUncertain ? "Uncertain" : IsTrue ? "True" : "False";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var s = obj as UncertainBoolean?;

            if (s == null)
                return false;

            return Equals(s.Value);
        }

        public override int GetHashCode()
        {
            return IntState;
        }

        public bool Equals(UncertainBoolean other)
        {
            return this == other;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("State", (int)_state);
        }

        public static implicit operator UncertainBoolean(bool value)
        {
            return value ? True : False;
        }

        public static implicit operator UncertainBoolean(bool? value)
        {
            return new UncertainBoolean(value == null ? State.Uncertain : (value.Value ? State.True : State.False));
        }

        public static bool operator ==(UncertainBoolean left, UncertainBoolean right)
        {
            return left.IntState == right.IntState;
        }

        public static bool operator !=(UncertainBoolean left, UncertainBoolean right)
        {
            return left.IntState != right.IntState;
        }

        public static UncertainBoolean operator &(UncertainBoolean left, UncertainBoolean right)
        {
            if (left.IsFalse || right.IsFalse)
                return False;

            return left.IsUncertain || right.IsUncertain ? Uncertain : True;
        }

        public static UncertainBoolean operator |(UncertainBoolean left, UncertainBoolean right)
        {
            if (left.IsTrue || right.IsTrue)
                return True;

            return left.IsUncertain || right.IsUncertain ? Uncertain : False;
        }

        public static UncertainBoolean operator ^(UncertainBoolean left, UncertainBoolean right)
        {
            return left.IsUncertain || right.IsUncertain ? Uncertain : (left == right ? False : True);
        }

        public static bool operator true(UncertainBoolean value)
        {
            return value.IsTrue;
        }

        public static bool operator false(UncertainBoolean value)
        {
            return value.IsFalse;
        }

        public static UncertainBoolean operator !(UncertainBoolean value)
        {
            return value.IsUncertain ? Uncertain : (value.IsTrue ? False : True);
        }

        private enum State
        {
            False = -1,
            Uncertain = 0,
            True = 0x7fffffff,
        }
    }
}
