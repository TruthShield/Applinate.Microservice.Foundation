// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;

    public abstract class InterceptorFactoryBase
    {
        [DebuggerHidden]
        public virtual Task<TResult> ExecuteAsync<TRequest, TResult>(
            ExecuteDelegate<TRequest, TResult> next,
            TRequest arg,
            CancellationToken cancellationToken)
            where TRequest : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus =>
            next(arg, cancellationToken) ?? Task.FromResult<TResult>(default);
    }
}