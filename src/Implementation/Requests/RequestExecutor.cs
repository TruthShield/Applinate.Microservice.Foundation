// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;
    internal class RequestExecutor : IRequestExecutor
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

        private static readonly Lazy<NestedDictionary<Type, Type, IRequestHandlerBuilder[]>> _RequestHandlers = new(RequestHandlerRegistryBuilder.BuildHandlerRegistry);

        private static NestedDictionary<Type, Type, IRequestHandlerBuilder[]> RequestHandlers => _RequestHandlers.Value;

        private static IRequestHandler<TArg, TResult> GetHandler<TArg, TResult>()
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

            if(factory.Skip(1).Any())
            {
                throw new InvalidOperationException("too many implementations"); // Undone: better error message here
            }

            var instance = factory.First().BuildRequestHandler<TArg, TResult>();

            return instance ?? throw ExceptionFactory.NoDefinedService<TArg, TResult>();
        }
    }
}