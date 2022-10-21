// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Test
{
    using Applinate;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class MockCommand.
    /// </summary>
    /// <typeparam name="TArg">The type of the t argument.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    public static class MockRequest<TArg, TResult>
        where TArg : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus
    {
        private static readonly AsyncLocal<MockCommandExecutor<TArg, TResult>> _Executor = new();
        private static readonly MockCommandExecutor<TArg, TResult> _GlobalExecutor = new();

        /// <summary>
        /// Gets a value indicating whether this instance is set.
        /// </summary>
        /// <value><c>true</c> if this instance is set; otherwise, <c>false</c>.</value>
        internal static bool IsSet =>
            (_Executor.Value is not null &&
            _Executor.Value.Behavior is not null) ||
            (_GlobalExecutor.Behavior is not null);

        //    //_GlobalExecutor.Behavior = (arg, cancellationToken) => Task.FromResult(behavior(arg));
        //}
        /// <summary>
        /// Clears the beharior for the duraton and context of the test.
        /// </summary>
        public static void Clear()
        {
            _GlobalExecutor.Behavior = null;

            if (_Executor.Value is null)
            {
                return;
            }

            _Executor.Value.Behavior = null;
        }

        //internal static void SetGlobally<TArg, TResult>(Func<TArg, TResult> behavior)
        //    where TArg : class, IReturn<TResult>
        //    where TResult : class
        //{
        //    _Executor.Value.Behavior = (arg, cancellationToken) => Task.FromResult(behavior(arg));
        /// <summary>
        /// Sets the specified behavior for the duration and context of the test.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public static void Set(Func<TArg, CancellationToken, Task<TResult>> behavior)
        {
            if (_Executor.Value is null)
            {
                _Executor.Value = new MockCommandExecutor<TArg, TResult>();
            }

            _Executor.Value.Behavior = behavior;
        }

        /// <summary>
        /// Sets the specified behavior for the duration and context of the test.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public static void SetForTestScope(Func<TArg, TResult> behavior)
        {
            if (_Executor.Value is null)
            {
                _Executor.Value = new MockCommandExecutor<TArg, TResult>();
            }

            _Executor.Value.Behavior = (arg, cancellationToken) => Task.FromResult(behavior(arg));
            // TODO: set commandhelper
        }

        public static void SetGlobally(Func<TArg, TResult> behavior)
        {
            _GlobalExecutor.Behavior = (arg, cancellationToken) => Task.FromResult(behavior(arg));
            // TODO: set commandhelper
        }

        internal static Task<TResult> Execute(TArg arg, CancellationToken cancellationToken)
        {
            if (_Executor?.Value?.Behavior is not null)
            {
                // TODO: throw if not set
                return _Executor.Value.Behavior(arg, cancellationToken);
            }

            if (_GlobalExecutor.Behavior is not null)
            {
                return _GlobalExecutor.Behavior(arg, cancellationToken);
            }

            throw new InvalidOperationException("mock behavior not set");
        }
    }
}