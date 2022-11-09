// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using Polly;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Helper class for executing commands implementing <see cref="IReturn{TResponse}"/>.
    /// </summary>
    public static class RequestHandlerProvider
    {
        /// <summary>
        /// Executes the specified command.  If possible the preferred usage is asynchronous as
        /// <see cref="ExecuteAsync{TResponse}(IReturn{TResponse}, CancellationToken)"/> which should be used
        /// when possible to avoid unnessasry blocking.
        /// </summary>
        /// <typeparam name="TResponse">The type of the t result.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>TResponse.</returns>
        [DebuggerStepThrough, DebuggerHidden]
        public static TResponse Execute<TResponse>(
            this IReturn<TResponse> command)
            where TResponse : class, IHaveResponseStatus =>
            AsyncHelper.RunSync<TResponse>(() => ExecuteAsync<TResponse>(command));

        /// <summary>
        /// Executes the specified command asynchronously.
        /// </summary>
        /// <typeparam name="TResponse">The type of the t result.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">
        /// The cancellation token that can be used by other objects or
        /// threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;TResponse&gt;.</returns>
        [DebuggerStepThrough, DebuggerHidden]
        public static Task<TResponse> ExecuteAsync<TResponse>(
            this IReturn<TResponse> command,
            CancellationToken cancellationToken = default)
            where TResponse : class, IHaveResponseStatus
        {
            return Policy.Handle<Exception>()
                .WaitAndRetry(new[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200) })
                .Execute(() =>
                {
                    var t = command.GetType();

                    var mi = typeof(RequestHandlerProvider).GetMethod(nameof(InternalExecuteAsync), BindingFlags.Static | BindingFlags.NonPublic);
                    var mi2 = mi?.MakeGenericMethod(command.GetType(), typeof(TResponse));
                    var result = mi2?.Invoke(null, new object[] { command, cancellationToken });

                    return result as Task<TResponse> ?? Task.FromResult<TResponse>(default);
                });
        }

        private static async Task<TResponse> InternalExecuteAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default)
            where TRequest : class, IReturn<TResponse>
            where TResponse : class, IHaveResponseStatus
        {
            var commandExecutor = ServiceProvider.Locate<IRequestExecutor>();
            var executorResult = await commandExecutor.ExecuteAsync<TRequest, TResponse>(request, cancellationToken).ConfigureAwait(false);
            return executorResult;
        }
    }
}