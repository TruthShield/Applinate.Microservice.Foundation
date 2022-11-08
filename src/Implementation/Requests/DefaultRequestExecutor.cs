// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;
    internal class DefaultRequestExecutor : IRequestHandler  
    {
        [DebuggerStepThrough]
        public async Task<TResult> ExecuteAsync<TArg, TResult>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus
        {
            var instance = RequestHandlerProvider.GetRegisteredHandler<TArg, TResult>();

            var result = await RequestInterceptorHelper<TArg, TResult>.Execute(
                instance,
                arg,
                cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}