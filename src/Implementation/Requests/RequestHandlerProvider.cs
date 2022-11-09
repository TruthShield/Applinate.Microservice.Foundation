// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using Polly;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Helper class for executing commands implementing <see cref="IReturn{TResult}"/>.
    /// </summary>
    public static class RequestHandlerProvider
    {
        /// <summary>
        /// Executes the specified command.  If possible the preferred usage is asynchronous as
        /// <see cref="ExecuteAsync{TResult}(IReturn{TResult}, CancellationToken)"/> which should be used
        /// when possible to avoid unnessasry blocking.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>TResult.</returns>
        [DebuggerStepThrough, DebuggerHidden]
        public static TResult Execute<TResult>(
            this IReturn<TResult> command)
            where TResult : class, IHaveRequestStatus =>
            AsyncHelper.RunSync<TResult>(() => ExecuteAsync<TResult>(command));

        /// <summary>
        /// Executes the specified command asynchronously.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">
        /// The cancellation token that can be used by other objects or
        /// threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;TResult&gt;.</returns>
        [DebuggerStepThrough, DebuggerHidden]
        public static Task<TResult> ExecuteAsync<TResult>(
            this IReturn<TResult> command,
            CancellationToken cancellationToken = default)
            where TResult : class, IHaveRequestStatus
        {
            return Policy.Handle<Exception>()
                .WaitAndRetry(new[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200) })
                .Execute(() =>
                {
                    var t = command.GetType();

                    var mi = typeof(RequestHandlerProvider).GetMethod(nameof(InternalExecuteAsync), BindingFlags.Static | BindingFlags.NonPublic);
                    var mi2 = mi?.MakeGenericMethod(command.GetType(), typeof(TResult));
                    var result = mi2?.Invoke(null, new object[] { command, cancellationToken });

                    return result as Task<TResult> ?? Task.FromResult<TResult>(default);
                });
        }

        private static async Task<TResult> InternalExecuteAsync<TRequest, TResult>(
            TRequest arg,
            CancellationToken cancellationToken = default)
            where TRequest : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus
        {
            var commandExecutor = ServiceProvider.Locate<IRequestExecutor>();
            var executorResult = await commandExecutor.ExecuteAsync<TRequest, TResult>(arg, cancellationToken).ConfigureAwait(false);
            return executorResult;
        }
    }
}