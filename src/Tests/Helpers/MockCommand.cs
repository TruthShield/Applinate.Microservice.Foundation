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
    public static class MockCommand

    {
        public static void Clear<TRequest>() where TRequest : class, IReturn<CommandResponse> =>
            MockRequest<TRequest, CommandResponse>.Clear();

        /// <summary>
        /// Sets the specified behavior for the duration and context of the test.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public static void SetForTestScope<TRequest>(Func<TRequest, CancellationToken, Task<CommandResponse>> behavior)
            where TRequest : class, IReturn<CommandResponse> =>
            MockRequest<TRequest, CommandResponse>.Set(behavior);

        /// <summary>
        /// Sets the specified behavior for the duration and context of the test.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public static void SetForTestScope<TRequest>(Func<TRequest, CommandResponse> behavior)
            where TRequest : class, IReturn<CommandResponse> =>
            MockRequest<TRequest, CommandResponse>.SetForTestScope(behavior);

        /// <summary>
        /// Sets for test scope.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public static void SetForTestScope<TRequest>(Action<TRequest> behavior)
            where TRequest : class, IReturn<CommandResponse> =>

            MockRequest<TRequest, CommandResponse>.SetForTestScope(
                a =>
                {
                    behavior(a);
                    return CommandResponse.Success;
                });
    }
}