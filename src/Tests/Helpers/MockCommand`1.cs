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
    public static class MockCommand

    {
        public static void Clear<TArg>() where TArg : class, IReturn<CommandResponse> =>
            MockRequest<TArg, CommandResponse>.Clear();

        /// <summary>
        /// Sets the specified behavior for the duration and context of the test.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public static void SetForTestScope<TArg>(Func<TArg, CancellationToken, Task<CommandResponse>> behavior)
            where TArg : class, IReturn<CommandResponse> =>
            MockRequest<TArg, CommandResponse>.Set(behavior);

        /// <summary>
        /// Sets the specified behavior for the duration and context of the test.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public static void SetForTestScope<TArg>(Func<TArg, CommandResponse> behavior)
            where TArg : class, IReturn<CommandResponse> =>
            MockRequest<TArg, CommandResponse>.SetForTestScope(behavior);

        /// <summary>
        /// Sets for test scope.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public static void SetForTestScope<TArg>(Action<TArg> behavior)
            where TArg : class, IReturn<CommandResponse> =>

            MockRequest<TArg, CommandResponse>.SetForTestScope(
                a =>
                {
                    behavior(a);
                    return CommandResponse.Success;
                });
    }
}