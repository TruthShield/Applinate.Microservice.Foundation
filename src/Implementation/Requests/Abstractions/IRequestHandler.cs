// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    /// <summary>
    /// A service that executes the specified <see cref="IReturn{TResult}"/>
    /// </summary>
    /// <typeparam name="TRequest">The type of the input argument.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IRequestHandler<TRequest, TResult> 
        where TRequest : class, IReturn<TResult> 
        where TResult : class, IHaveRequestStatus
    {
        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>
        /// <param name="arg">The input argument.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;TResult&gt;.</returns>
        Task<TResult> ExecuteAsync(TRequest arg, CancellationToken cancellationToken = default);
    }
}