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
    /// <typeparam name="TRequest">The type of the t argument.</typeparam>
    /// <typeparam name="TResponse">The type of the t result.</typeparam>
    public static class MockRequest<TRequest, TResponse>
        where TRequest : class, IReturn<TResponse>
        where TResponse : class, IHaveResponseStatus
    {
        private static readonly AsyncLocal<MockCommandExecutor<TRequest, TResponse>> _Executor = new();
        private static readonly MockCommandExecutor<TRequest, TResponse> _GlobalExecutor = new();

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

        //internal static void SetGlobally<TRequest, TResponse>(Func<TRequest, TResponse> behavior)
        //    where TRequest : class, IReturn<TResponse>
        //    where TResponse : class
        //{
        //    _Executor.Value.Behavior = (arg, cancellationToken) => Task.FromResult(behavior(arg));
        /// <summary>
        /// Sets the specified behavior for the duration and context of the test.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public static void Set(Func<TRequest, CancellationToken, Task<TResponse>> behavior)
        {
            if (_Executor.Value is null)
            {
                _Executor.Value = new MockCommandExecutor<TRequest, TResponse>();
            }

            _Executor.Value.Behavior = behavior;
        }

        /// <summary>
        /// Sets the specified behavior for the duration and context of the test.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public static void SetForTestScope(Func<TRequest, TResponse> behavior)
        {
            if (_Executor.Value is null)
            {
                _Executor.Value = new MockCommandExecutor<TRequest, TResponse>();
            }

            _Executor.Value.Behavior = (arg, cancellationToken) => Task.FromResult(behavior(arg));
            // TODO: set requesthelper
        }

        public static void SetGlobally(Func<TRequest, TResponse> behavior)
        {
            _GlobalExecutor.Behavior = (arg, cancellationToken) => Task.FromResult(behavior(arg));
            // TODO: set requesthelper
        }

        internal static Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken)
        {
            if (_Executor?.Value?.Behavior is not null)
            {
                // TODO: throw if not set
                return _Executor.Value.Behavior(request, cancellationToken);
            }

            if (_GlobalExecutor.Behavior is not null)
            {
                return _GlobalExecutor.Behavior(request, cancellationToken);
            }

            throw new InvalidOperationException("mock behavior not set");
        }
    }
}