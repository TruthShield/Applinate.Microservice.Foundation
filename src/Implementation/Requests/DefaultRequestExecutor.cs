// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;
    internal class DefaultRequestExecutor : IExecuteRequest
    {
        public async Task<TResult> ExecuteAsync<TArg, TResult>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus
        {
            var instance = GetHandler<TArg, TResult>();

            var result = await RequestInterceptorHelper<TArg, TResult>.Execute(
                instance,
                arg,
                cancellationToken).ConfigureAwait(false);

            return result;
        }

        private static readonly Lazy<NestedDictionary<Type, Type, IRequestHandlerRegistry>> _RequestHandlers = new(RequestHandlerRegistryBuilder.BuildRegistry);

        private static NestedDictionary<Type, Type, IRequestHandlerRegistry> RequestHandlers => _RequestHandlers.Value;

        private static IHandleRequest<TArg, TResult> GetHandler<TArg, TResult>()
        where TArg : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus
        {
            var key1 = typeof(TArg);
            var key2 = typeof(TResult);

            if (!RequestHandlers.ContainsKey(key1, key2))
            {
                // fault on execution because the behavior may be overriden by an interceptor
                return new FaultGeneratingCommandExecutor<TArg, TResult>(() =>
                    ExceptionFactory.NoDefinedService<TArg, TResult>());
            }

            var factory = RequestHandlers[key1][key2];

            var instance = factory.GetRequestHandler<TArg, TResult>();

            return instance ?? throw ExceptionFactory.NoDefinedService<TArg, TResult>();
        }
    }
}