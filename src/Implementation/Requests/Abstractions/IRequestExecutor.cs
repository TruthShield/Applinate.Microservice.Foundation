// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    internal interface IRequestExecutor
    {
        Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class, IReturn<TResponse>
            where TResponse : class, IHaveResponseStatus;
    }
}