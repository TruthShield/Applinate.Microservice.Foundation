// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    internal interface IHandlerFactory
    {
        IHandleRequest<TArg1, TResult1> Build<TArg1, TResult1>()
            where TArg1 : class, IReturn<TResult1>
            where TResult1 : class, IHaveRequestStatus;
    }
}