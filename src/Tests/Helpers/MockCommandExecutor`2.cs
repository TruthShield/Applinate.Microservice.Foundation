// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Test
{
    using Applinate;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class MockCommandExecutor. This class cannot be inherited.
    /// Implements the <see cref="Applinate.IHandleRequest{TArg, TResult}" />
    /// </summary>
    /// <typeparam name="TArg">The type of the t argument.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <seealso cref="Applinate.IHandleRequest{TArg, TResult}" />
    [BypassSafetyChecks]
    internal sealed class MockCommandExecutor<TArg, TResult> : IHandleRequest<TArg, TResult>
        where TArg : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus
    {
        public Func<TArg, CancellationToken, Task<TResult>>? Behavior;

        public Task<TResult> ExecuteAsync(TArg arg, CancellationToken cancellationToken = default)
        {
            if (Behavior is not null)
            {
                return Behavior(arg, cancellationToken);
            }

            throw new InvalidOperationException("should not have called this imlementation, it is not set"); // TODO: better error message
        }
    }
}