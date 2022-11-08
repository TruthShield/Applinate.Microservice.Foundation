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
        private static readonly Lazy<NestedDictionary<Type, Type, IHandlerFactory>> _RequestHandlerFactoryRegistry = new(RequestHandlerFactoryRegistryBuilder.BuildRegistry);

        private static NestedDictionary<Type, Type, IHandlerFactory> RequestHandlerFactoryRegistry => _RequestHandlerFactoryRegistry.Value;

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
        public static Task<TResult?> ExecuteAsync<TResult>(
            this IReturn<TResult> command,
            CancellationToken cancellationToken = default)
            where TResult : class, IHaveRequestStatus
        {
            return Policy.Handle<Exception>()
                .WaitAndRetry(new[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200) })
                .Execute(() =>
                {
                    var t = command.GetType();

                    var mi =  typeof(RequestHandlerProvider).GetMethod(nameof(InternalExecuteAsync), BindingFlags.Static | BindingFlags.NonPublic);
                    var mi2 = mi?.MakeGenericMethod(command.GetType(), typeof(TResult));
                    var result = mi2?.Invoke(null, new object[] { command, cancellationToken });

                    return result as Task<TResult?> ?? Task.FromResult(default(TResult));
                });
        }

        private static async Task<TResult> InternalExecuteAsync<TArg, TResult>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus
        {
            var commandExecutor = ServiceProvider.Locate<IRequestHandler>();
            var executorResult = await commandExecutor.ExecuteAsync<TArg, TResult>(arg, cancellationToken).ConfigureAwait(false);
            return executorResult;
        }

        internal static Task<TResult> Ex<TRequest, TResult>(TRequest request, CancellationToken cancellationToken)
            where TRequest : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus =>
            GetRegisteredHandler<TRequest, TResult>().ExecuteAsync(request, cancellationToken);

        internal static IHandleRequest<TArg, TResult> GetRegisteredHandler<TArg, TResult>()
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus
        {
            var key1 = typeof(TArg);
            var key2 = typeof(TResult);

            if (!RequestHandlerFactoryRegistry.ContainsKey(key1, key2))
            {
                // fault on execution because the behavior may be overriden by an interceptor
                return new FaultGeneratingCommandExecutor<TArg, TResult>(() =>
                    ExceptionFactory.NoDefinedService<TArg, TResult>());
            }

            var factory = RequestHandlerFactoryRegistry[key1][key2];

            var instance = factory.Build<TArg, TResult>();

            return instance ?? throw ExceptionFactory.NoDefinedService<TArg, TResult>();
        }
    }
}