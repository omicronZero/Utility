using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Utility.Contexts
{
    public static class ThreadContextual<TContext>
        where TContext : class
    {
        private static readonly object SyncRoot;

        [ThreadStatic]
        private static Stack<(long Generation, TContext Context)> _threadContexts;

        [ThreadStatic]
        private static TContext _currentContext;

        //used to ensure that ContextHandles are not disposed outside of their allocating thread
        //if the handle on a constructing thread is not equal to the thread local handle of the
        //disposing thread, an exception is thrown. The local thread handle is obtained from
        //_threadHandles upon the first call of ThreadLocalHandle
        //(_threadLocalHandle == 0 upon the first call)
        [ThreadStatic]
        private static long _threadLocalHandle;

        //Used to prevent disposing contexts multiple times or disposing the wrong context
        private static long _currentGeneration;
        private static long _threadHandles;

        private static Stack<(long Generation, TContext Context)> ThreadContexts
        {
            get
            {
                var contexts = _threadContexts;

                if (contexts == null)
                {
                    contexts = new Stack<(long Generation, TContext Context)>();
                    _threadContexts = contexts;
                }

                return contexts;
            }
        }

        static ThreadContextual()
        {
            SyncRoot = new object();
            _threadContexts = new Stack<(long Generation, TContext Context)>();
        }

        private static long ThreadLocalHandle
        {
            get
            {
                if (_threadLocalHandle == 0)
                    _threadLocalHandle = Interlocked.Increment(ref _threadHandles);

                return _threadLocalHandle;
            }
        }

        /// <summary>
        /// Gets the current context or the default context if the current context is null or has not been set specifically. See <see cref="GetDefaultContext"/> for further information about default contexts.
        /// </summary>
        public static TContext CurrentContext
        {
            get
            {
                TContext context = _currentContext;

                if (context == null)
                {
                    context = GlobalContext<TContext>.CurrentContext;
                }

                return context;
            }
        }

        /// <summary>
        /// Gets the thread's current context. If no context has been specified for the current thread, creates a fallback to the global context if <paramref name="fallbackToGlobal"/> is true
        /// or returns null otherwise.
        /// </summary>
        /// <param name="fallbackToGlobal">Determines whether to use the context defined in <see cref="GlobalContext{TContext}"/> if no thread local context has been specified or the specified context is null.</param>
        /// <returns></returns>
        public static TContext GetCurrentContext(bool fallbackToGlobal) => fallbackToGlobal ? _currentContext ?? GlobalContext<TContext>.CurrentContext : _currentContext;

        /// <summary>
        /// Gets the thread's current context. If no context has been specified for the current thread, creates a fallback to the global context if <paramref name="fallbackToGlobal"/> is true
        /// or returns null otherwise.
        /// </summary>
        /// <param name="fallbackToGlobal"></param>
        /// <param name="globalContextFallbackToDefault">Indicates whether to resolve the default context if no global context has been specified.</param>
        /// <returns></returns>
        public static TContext GetThreadContext(bool fallbackToGlobal, bool globalContextFallbackToDefault) => fallbackToGlobal ? _currentContext ?? GlobalContext<TContext>.GetCurrentContext(globalContextFallbackToDefault) : _currentContext;

        public static ContextHandle Push(TContext context)
        {
            long gen = Interlocked.Increment(ref _currentGeneration);

            ThreadContexts.Push((gen, context));
            _currentContext = context;

            return new ContextHandle(ThreadLocalHandle, gen, ThreadContexts);
        }

        public struct ContextHandle : IDisposable
        {
            private readonly long _contextThreadLocalHandle;
            private readonly long _generation;
            private readonly Stack<(long Generation, TContext Context)> _contexts;

            internal ContextHandle(long contextThreadLocalHandle, long generation, Stack<(long, TContext)> contexts)
            {
                _contextThreadLocalHandle = contextThreadLocalHandle;
                _generation = generation;
                _contexts = contexts;
            }

            public void Pop()
            {
                if (_contextThreadLocalHandle != _threadLocalHandle)
                    throw new InvalidOperationException("Context handle was created on another thread.");

                if (_contexts.Peek().Generation != _generation)
                {
                    foreach (var e in _contexts)
                    {
                        if (e.Generation == _generation) //other contexts were pushed beforehand
                            throw new InvalidOperationException("The current context handle does not refer to the currently active context.");
                    }

                    return; //redundant calls to Dispose for the same handle
                }

                _contexts.Pop();

                _currentContext = _contexts.Count == 0 ? null : _contexts.Peek().Context;
            }

            void IDisposable.Dispose()
            {
                Pop();
            }
        }
    }
}