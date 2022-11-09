// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    internal interface IRequestHandlerBuilder
    {
        IRequestHandler<TRequest1, TResponse1> BuildRequestHandler<TRequest1, TResponse1>()
            where TRequest1 : class, IReturn<TResponse1>
            where TResponse1 : class, IHaveResponseStatus;
    }
}