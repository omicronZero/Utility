using System;

namespace Utility.Dynamic
{
    /// <summary>
    /// Defines a mechanism that allows the call delegation by interface delegates.
    /// </summary>
    public interface IDelegationUnit
    {
        /// <summary>
        /// Invokes a method using a delegation unit as a surrogate.
        /// </summary>
        /// <param name="type">The interface type underlying the interface delegate.</param>
        /// <param name="method">The method that was called on the interface delegate</param>
        /// <param name="argumentTypes">The generic arguments passed to the called method.</param>
        /// <param name="state">The state that was set on the interface delegate instantiation.</param>
        /// <param name="parameters">The parameters, the interface delegate method was called with.</param>
        /// <returns>The value produced by the call.</returns>
        object Invoke(Type type, System.Reflection.MethodInfo method, Type[] argumentTypes, object state, object[] parameters);
    }
}
