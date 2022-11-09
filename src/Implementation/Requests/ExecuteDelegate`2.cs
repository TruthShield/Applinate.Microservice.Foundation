// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public delegate Task<TResponse> ExecuteDelegate<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken)
        where TRequest : class, IReturn<TResponse>
        where TResponse : class, IHaveResponseStatus;

}