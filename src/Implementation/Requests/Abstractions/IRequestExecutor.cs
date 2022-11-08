// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    internal interface IRequestExecutor
    {
        Task<TResult> ExecuteAsync<TArg, TResult>(TArg arg, CancellationToken cancellationToken = default)
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus;
    }
}