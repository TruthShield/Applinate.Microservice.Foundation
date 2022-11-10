// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;

    public class InterceptorBase<TRequest, TResponse>
        where TRequest : class, IReturn<TResponse>
        where TResponse : class, IHaveResponseStatus
    {
        private readonly ExecuteDelegate<TRequest, TResponse> _Core;

        [DebuggerHidden]
        public InterceptorBase(ExecuteDelegate<TRequest, TResponse> core)
            => _Core = core;

        /// <summary>
        /// Execute as an asynchronous operation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to
        /// receive notice of cancellation.</param>
        /// <returns>A Task&lt;TResponse&gt; representing the asynchronous operation.</returns>
        [DebuggerHidden]
        public virtual async Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken)
        {
            var newArg = await PreProcessAsync(request).ConfigureAwait(false);
            var result = await ExecuteCoreAsync(newArg, cancellationToken).ConfigureAwait(false);
            var newResult = await PostProcessAsync(result).ConfigureAwait(false);
            return newResult;
        }

        [DebuggerHidden]
        private Task<TResponse> ExecuteCoreAsync(TRequest request, CancellationToken cancellationToken)
        {
            return _Core(request, cancellationToken);
        }

        /// <summary>
        /// A hook to pre-process the call.  Usually used for logging, tracing, or modifying the input argumment.
        /// 
        /// If you need to modify behavior use <see cref="ExecuteCoreAsync(TRequest, CancellationToken)"/>.        /// </summary>
        /// <param name="response">The result.</param>
        /// <returns>Task&lt;TResponse&gt;.</returns>
        [DebuggerHidden]
        protected virtual Task<TResponse> PostProcessAsync(TResponse response)
        {
            return Task.FromResult(response);
        }

        /// A hook to post-process the call.  Usually used for logging, tracing purposes, or modification of the output.
        /// If you need to modify behavior use <see cref="ExecuteCoreAsync(TRequest, CancellationToken)"/>.
        [DebuggerHidden]
        protected virtual Task<TRequest> PreProcessAsync(TRequest request)
        {
            return Task.FromResult(request);
        }
    }
}