// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;
    internal class RequestExecutor : IRequestExecutor
    {
        public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default)
            where TRequest : class, IReturn<TResponse>
            where TResponse : class, IHaveResponseStatus
        {
            var instance = GetHandler<TRequest, TResponse>();

            var result = await RequestInterceptorHelper<TRequest, TResponse>.Execute(
                instance,
                request,
                cancellationToken).ConfigureAwait(false);

            return result;
        }

        private static readonly Lazy<NestedDictionary<Type, Type, IRequestHandlerBuilder[]>> _RequestHandlers = new(RequestHandlerRegistryBuilder.BuildHandlerRegistry);

        private static NestedDictionary<Type, Type, IRequestHandlerBuilder[]> RequestHandlers => _RequestHandlers.Value;

        private static IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>()
        where TRequest : class, IReturn<TResponse>
        where TResponse : class, IHaveResponseStatus
        {
            var key1 = typeof(TRequest);
            var key2 = typeof(TResponse);

            if (!RequestHandlers.ContainsKey(key1, key2))
            {
                // fault on execution because the behavior may be overriden by an interceptor
                return new FaultGeneratingCommandExecutor<TRequest, TResponse>(() =>
                    ExceptionFactory.NoDefinedService<TRequest, TResponse>());
            }

            var factory = RequestHandlers[key1][key2];

            if(factory.Skip(1).Any())
            {
                throw new InvalidOperationException("too many implementations"); // Undone: better error message here
            }

            var instance = factory.First().BuildRequestHandler<TRequest, TResponse>();

            return instance ?? throw ExceptionFactory.NoDefinedService<TRequest, TResponse>();
        }
    }
}