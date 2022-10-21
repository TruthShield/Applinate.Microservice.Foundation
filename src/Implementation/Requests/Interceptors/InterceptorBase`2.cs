﻿// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Diagnostics;

    public class InterceptorBase<TArg, TResult>
        where TArg : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus
    {
        private readonly ExecuteDelegate<TArg, TResult> _Core;

        [DebuggerHidden]
        public InterceptorBase(ExecuteDelegate<TArg, TResult> core)
            => _Core = core;

        /// <summary>
        /// Execute as an asynchronous operation.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to
        /// receive notice of cancellation.</param>
        /// <returns>A Task&lt;TResult&gt; representing the asynchronous operation.</returns>
        [DebuggerHidden]
        public virtual async Task<TResult> ExecuteAsync(TArg arg, CancellationToken cancellationToken)
        {
            var newArg = await PreProcessAsync(arg).ConfigureAwait(false);
            var result = await ExecuteCoreAsync(newArg, cancellationToken).ConfigureAwait(false);
            var newResult = await PostProcessAsync(result).ConfigureAwait(false);
            return newResult;
        }

        [DebuggerHidden]
        private Task<TResult> ExecuteCoreAsync(TArg arg, CancellationToken cancellationToken)
        {
            return _Core(arg, cancellationToken);
        }

        /// <summary>
        /// A hook to pre-process the call.  Usually used for logging, tracing, or modifying the input argumment.
        /// 
        /// If you need to modify behavior use <see cref="ExecuteCoreAsync(TArg, CancellationToken)"/>.        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Task&lt;TResult&gt;.</returns>
        [DebuggerHidden]
        protected virtual Task<TResult> PostProcessAsync(TResult result)
        {
            return Task.FromResult(result);
        }

        /// A hook to post-process the call.  Usually used for logging, tracing purposes, or modification of the output.
        /// If you need to modify behavior use <see cref="ExecuteCoreAsync(TArg, CancellationToken)"/>.
        [DebuggerHidden]
        protected virtual Task<TArg> PreProcessAsync(TArg arg)
        {
            return Task.FromResult(arg);
        }
    }
}