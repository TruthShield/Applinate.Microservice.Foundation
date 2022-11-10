// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    /// <summary>
    /// A service that executes the specified <see cref="IReturn{TResponse}"/>
    /// </summary>
    /// <typeparam name="TRequest">The type of the input argument.</typeparam>
    /// <typeparam name="TResponse">The type of the result.</typeparam>
    public interface IRequestHandler<TRequest, TResponse> 
        where TRequest : class, IReturn<TResponse> 
        where TResponse : class, IHaveResponseStatus
    {
        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>
        /// <param name="request">The input argument.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;TResponse&gt;.</returns>
        Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}