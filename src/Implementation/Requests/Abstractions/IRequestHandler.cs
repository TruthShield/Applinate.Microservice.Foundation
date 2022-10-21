// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public interface IRequestHandler
    {
        Task<TResult> ExecuteAsync<TArg, TResult>(TArg arg, CancellationToken cancellationToken = default)
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus;
    }
}