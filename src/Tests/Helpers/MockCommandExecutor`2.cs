// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Test
{
    using Applinate;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class MockCommandExecutor. This class cannot be inherited.
    /// Implements the <see cref="Applinate.IRequestHandler{TRequest, TResult}" />
    /// </summary>
    /// <typeparam name="TRequest">The type of the t argument.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <seealso cref="Applinate.IRequestHandler{TRequest, TResult}" />
    [BypassSafetyChecks]
    internal sealed class MockCommandExecutor<TRequest, TResult> : IRequestHandler<TRequest, TResult>
        where TRequest : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus
    {
        public Func<TRequest, CancellationToken, Task<TResult>>? Behavior;

        public Task<TResult> ExecuteAsync(TRequest arg, CancellationToken cancellationToken = default)
        {
            if (Behavior is not null)
            {
                return Behavior(arg, cancellationToken);
            }

            throw new InvalidOperationException("should not have called this imlementation, it is not set"); // TODO: better error message
        }
    }
}