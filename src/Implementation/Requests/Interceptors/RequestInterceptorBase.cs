// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Threading;

    // used for auto-registration
    /// <summary>
    /// Class CommandInterceptorBase.
    /// 
    /// This is a marker class used specifically to auto-register the interceptor.
    /// 
    /// If you want manual interception, just use the base class <see cref="InterceptorBase{TArg, TResult}"/>
    /// 
    /// Implements the <see cref="InterceptorBase{TArg, TResult}" />
    /// </summary>
    /// <typeparam name="TArg">The type of the t argument.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <seealso cref="Applinate.InterceptorBase{TArg, TResult}" />
    /// <remarks>
    /// You can use <see="CommaandInterceptorAttribute" /> to determine the order of interceptors
    /// </remarks>
    public class RequestInterceptorBase<TArg, TResult> : InterceptorBase<TArg, TResult>
        where TArg : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus
    {
        public RequestInterceptorBase(ExecuteDelegate<TArg, TResult> core) : base(core)
        {
        }

        /// <summary>
        /// point to override the overall execution without affectig <see cref="M:Applinate.InterceptorBase`2.PreProcessAsync(`0)" />
        /// or <see cref="M:Applinate.InterceptorBase`2.PostProcessAsync(`1)" />.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;TResult&gt;.</returns>
        public override Task<TResult> ExecuteAsync(TArg arg, CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(arg, cancellationToken);
        }

        /// <summary>
        /// A hook to pre-process the call.  Usually used for logging, tracing, or modifying the input argumment.
        /// If you need to modify behavior use <see cref="M:Applinate.InterceptorBase`2.ExecuteCoreAsync(`0,System.Threading.CancellationToken)" />.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Task&lt;TResult&gt;.</returns>
        protected override Task<TResult> PostProcessAsync(TResult result)
        {
            return base.PostProcessAsync(result);
        }

        /// <summary>
        /// Pres the process asynchronous.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>Task&lt;TArg&gt;.</returns>
        /// A hook to post-process the call.  Usually used for logging, tracing purposes, or modification of the output.
        /// If you need to modify behavior use <see cref="M:Applinate.InterceptorBase`2.ExecuteCoreAsync(`0,System.Threading.CancellationToken)" />.
        protected override Task<TArg> PreProcessAsync(TArg arg)
        {
            return base.PreProcessAsync(arg);
        }
    }
}