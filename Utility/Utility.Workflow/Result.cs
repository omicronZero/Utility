using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    /*
     * I initially implemented an empty struct FailedResult that worked together with result. An
     * implicit conversion operator together with two equality operators compared the Result<T>
     * instances and the nullable FailedResult? and simply returned Success or !Success from the
     * Result<T> instance respectively. You could then just call
     * 
     * result == null
     * 
     * etc. However, for Ts that are, themselves, nullable, this will not be nice. We would have
     * to have the constraint T : notnull but we would also like to support reference types as T.
     * A separate Result type for structs is not desirable as it would not gain us sufficient
     * benefits to justify the addition of complexity.
     */

    public struct Result<T> : IEquatable<Result<T>>
    {
        public static Result<T> Failed => new Result<T>();

        public bool Success { get; }
        private readonly T _result;

        public Result(T value)
        {
            Success = true;
            _result = value;
        }

        public override string ToString()
        {
            return Success ? (_result?.ToString() ?? string.Empty) : "<Failed>";
        }

        public T Value
        {
            get
            {
                if (!Success)
                    throw new InvalidOperationException("No result available.");

                return _result;
            }
        }

        public bool TryGetResult(out T value)
        {
            if (Success)
            {
                value = _result;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public bool Equals(Result<T> other)
        {
            if (!Success)
            {
                return !other.Success;
            }
            else if (!other.Success)
            {
                return false;
            }

            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Result<T> r && Equals(r);
        }

        public override int GetHashCode()
        {
            return Success ? ~(Value?.GetHashCode() ?? 0) : 0;
        }

        public static implicit operator Result<T>(T value)
        {
            return new Result<T>(value);
        }

        public static explicit operator T(Result<T> result)
        {
            return result.Value;
        }

        public static bool operator ==(Result<T> left, Result<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Result<T> left, Result<T> right)
        {
            return !left.Equals(right);
        }

        public static bool operator true(Result<T> result)
        {
            return result.Success;
        }

        public static bool operator false(Result<T> result)
        {
            return !result.Success;
        }
    }
}
