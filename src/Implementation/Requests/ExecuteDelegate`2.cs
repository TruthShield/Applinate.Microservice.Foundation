// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public delegate Task<TResult?> ExecuteDelegate<TArg, TResult>(
            TArg arg,
            CancellationToken cancellationToken)
        where TArg : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus;

}