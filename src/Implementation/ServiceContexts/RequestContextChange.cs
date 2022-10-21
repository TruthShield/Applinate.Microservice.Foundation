// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public sealed record RequestContextChange
    {
        public RequestContextChange(
            RequestContextChangeType changeType, 
            Type commandType, 
            Type returnType, 
            int serviceCallCount)
        {
            ContextChangeType = changeType;
            CommandType       = commandType;
            ReturnType        = returnType;
            ServiceCallCount  = serviceCallCount;
        }

        public RequestContextChangeType ContextChangeType { get; }
        public Type CommandType { get; }
        public Type ReturnType { get; }
        public int ServiceCallCount { get; }

        public override string ToString()
        {
            return $@"{ContextChangeType} {ServiceCallCount} {CommandType}";
        }

        public static RequestContextChange Entry<TArg, TResult>(int serviceCallCount)
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus =>
            new(RequestContextChangeType.Entry, typeof(TArg), typeof(TResult), serviceCallCount);

        public static RequestContextChange Exit<TArg, TResult>(int serviceCallCount)
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus =>
            new(RequestContextChangeType.Exit, typeof(TArg), typeof(TResult), serviceCallCount);
    }
}