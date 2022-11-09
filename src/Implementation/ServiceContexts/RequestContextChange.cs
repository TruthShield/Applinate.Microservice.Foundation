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

        public static RequestContextChange Entry<TRequest, TResult>(int serviceCallCount)
            where TRequest : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus =>
            new(RequestContextChangeType.Entry, typeof(TRequest), typeof(TResult), serviceCallCount);

        public static RequestContextChange Exit<TRequest, TResult>(int serviceCallCount)
            where TRequest : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus =>
            new(RequestContextChangeType.Exit, typeof(TRequest), typeof(TResult), serviceCallCount);
    }
}