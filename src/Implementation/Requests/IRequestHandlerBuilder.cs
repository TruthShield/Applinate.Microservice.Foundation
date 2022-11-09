// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    internal interface IRequestHandlerBuilder
    {
        IRequestHandler<TRequest1, TResult1> BuildRequestHandler<TRequest1, TResult1>()
            where TRequest1 : class, IReturn<TResult1>
            where TResult1 : class, IHaveRequestStatus;
    }
}