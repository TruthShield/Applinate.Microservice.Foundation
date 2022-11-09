// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public delegate Task<TResult> ExecuteDelegate<TRequest, TResult>(
            TRequest arg,
            CancellationToken cancellationToken)
        where TRequest : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus;

}