// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    internal interface IRequestExecutor
    {
        Task<TResult> ExecuteAsync<TRequest, TResult>(TRequest arg, CancellationToken cancellationToken = default)
            where TRequest : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus;
    }
}