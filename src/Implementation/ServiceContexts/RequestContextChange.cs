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

        public static RequestContextChange Entry<TRequest, TResponse>(int serviceCallCount)
            where TRequest : class, IReturn<TResponse>
            where TResponse : class, IHaveResponseStatus =>
            new(RequestContextChangeType.Entry, typeof(TRequest), typeof(TResponse), serviceCallCount);

        public static RequestContextChange Exit<TRequest, TResponse>(int serviceCallCount)
            where TRequest : class, IReturn<TResponse>
            where TResponse : class, IHaveResponseStatus =>
            new(RequestContextChangeType.Exit, typeof(TRequest), typeof(TResponse), serviceCallCount);
    }
}