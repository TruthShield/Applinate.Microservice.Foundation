// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;

    public abstract class InterceptorFactoryBase
    {
        [DebuggerHidden]
        public virtual Task<TResponse> ExecuteAsync<TRequest, TResponse>(
            ExecuteDelegate<TRequest, TResponse> next,
            TRequest request,
            CancellationToken cancellationToken)
            where TRequest : class, IReturn<TResponse>
            where TResponse : class, IHaveResponseStatus =>
            next(request, cancellationToken) ?? Task.FromResult<TResponse>(default);
    }
}