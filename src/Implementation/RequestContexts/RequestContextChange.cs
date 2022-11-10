// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public sealed record RequestContextChange
    {
        public RequestContextChange(
            RequestContextChangeType changeType, 
            Type requestType, 
            Type responseType, 
            int serviceCallCount)
        {
            ContextChangeType = changeType;
            RequestType       = requestType;
            ResponseType      = responseType;
            ServiceCallCount  = serviceCallCount;
        }

        public RequestContextChangeType ContextChangeType { get; }
        public Type RequestType { get; }
        public Type ResponseType { get; }
        public int ServiceCallCount { get; }

        public override string ToString()
        {
            return $@"{ContextChangeType} {ServiceCallCount} {RequestType}";
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