// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;
    internal class RequestExecutor : IRequestExecutor
    {
        public async Task<TResult> ExecuteAsync<TRequest, TResult>(
            TRequest arg,
            CancellationToken cancellationToken = default)
            where TRequest : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus
        {
            var instance = GetHandler<TRequest, TResult>();

            var result = await RequestInterceptorHelper<TRequest, TResult>.Execute(
                instance,
                arg,
                cancellationToken).ConfigureAwait(false);

            return result;
        }

        private static readonly Lazy<NestedDictionary<Type, Type, IRequestHandlerBuilder[]>> _RequestHandlers = new(RequestHandlerRegistryBuilder.BuildHandlerRegistry);

        private static NestedDictionary<Type, Type, IRequestHandlerBuilder[]> RequestHandlers => _RequestHandlers.Value;

        private static IRequestHandler<TRequest, TResult> GetHandler<TRequest, TResult>()
        where TRequest : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus
        {
            var key1 = typeof(TRequest);
            var key2 = typeof(TResult);

            if (!RequestHandlers.ContainsKey(key1, key2))
            {
                // fault on execution because the behavior may be overriden by an interceptor
                return new FaultGeneratingCommandExecutor<TRequest, TResult>(() =>
                    ExceptionFactory.NoDefinedService<TRequest, TResult>());
            }

            var factory = RequestHandlers[key1][key2];

            if(factory.Skip(1).Any())
            {
                throw new InvalidOperationException("too many implementations"); // Undone: better error message here
            }

            var instance = factory.First().BuildRequestHandler<TRequest, TResult>();

            return instance ?? throw ExceptionFactory.NoDefinedService<TRequest, TResult>();
        }
    }
}