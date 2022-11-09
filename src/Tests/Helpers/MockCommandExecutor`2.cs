// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Test
{
    using Applinate;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class MockCommandExecutor. This class cannot be inherited.
    /// Implements the <see cref="Applinate.IRequestHandler{TRequest, TResponse}" />
    /// </summary>
    /// <typeparam name="TRequest">The type of the t argument.</typeparam>
    /// <typeparam name="TResponse">The type of the t result.</typeparam>
    /// <seealso cref="Applinate.IRequestHandler{TRequest, TResponse}" />
    [BypassSafetyChecks]
    internal sealed class MockCommandExecutor<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IReturn<TResponse>
        where TResponse : class, IHaveResponseStatus
    {
        public Func<TRequest, CancellationToken, Task<TResponse>>? Behavior;

        public Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            if (Behavior is not null)
            {
                return Behavior(request, cancellationToken);
            }

            throw new InvalidOperationException("should not have called this imlementation, it is not set"); // TODO: better error message
        }
    }
}