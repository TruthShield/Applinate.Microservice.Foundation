// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;

    public abstract class InterceptorFactoryBase
    {
        [DebuggerHidden]
        public virtual Task<TResult> ExecuteAsync<TArg, TResult>(
            ExecuteDelegate<TArg, TResult> next,
            TArg arg,
            CancellationToken cancellationToken)
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus =>
            next(arg, cancellationToken) ?? Task.FromResult<TResult>(default);
    }
}