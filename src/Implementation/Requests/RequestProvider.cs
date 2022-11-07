// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using Polly;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Helper class for executing commands implementing <see cref="IReturn{TResult}"/>.
    /// </summary>
    public static class RequestProvider
    {
        private static readonly object _SyncLock = new object();
        private static readonly NestedDictionary<Type, Type, Type> CommandHandlerTypeRegistry = new NestedDictionary<Type, Type, Type>();

        private static bool _IsInitialized;

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
                .Execute(() => { 
                    var t = command.GetType();

                    var mi = typeof(RequestProvider).GetMethod(nameof(InternalExecuteAsync), BindingFlags.Static | BindingFlags.NonPublic);
                    var mi2 = mi?.MakeGenericMethod(t, typeof(TResult));
                    var result = mi2?.Invoke(null, new object[] { command, cancellationToken });

                    return result as Task<TResult?> ?? Task.FromResult(default(TResult));
                });
        }

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
            where TResult: class, IHaveRequestStatus => 
            AsyncHelper.RunSync<TResult>(() => ExecuteAsync<TResult>(command));

        internal static IHandleRequest<TArg, TResult> GetRegisteredHandler<TArg, TResult>()
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus
        {
            var key1 = typeof(TArg);
            var key2 = typeof(TResult);

            if (!CommandHandlerTypeRegistry.ContainsKey(key1, key2))
            {
                // fault on execution because the behavior may be overriden by an interceptor
                return new FaultGeneratingCommandExecutor<TArg, TResult>(() =>
                    ExceptionFactory.NoDefinedService<TArg, TResult>());
            }

            var type = CommandHandlerTypeRegistry[key1][key2];

            var instance = Activator.CreateInstance(type) as IHandleRequest<TArg, TResult>;

            return instance ?? throw ExceptionFactory.NoDefinedService<TArg, TResult>();
        }

       
        [DebuggerHidden]
        private static void Initialize()
        {
            if (_IsInitialized)
            {
                return;
            }

            lock (_SyncLock)
            {
                if (_IsInitialized)
                {
                    return;
                }

                var q = TypeRegistry.Types.Where(x => x.IsClass).ToArray();

                PopulateRegistry(q);

                _IsInitialized = true;
            }
        }

        [DebuggerHidden]
        private static async Task<TResult> InternalExecuteAsync<TArg, TResult>(
            TArg arg,
            CancellationToken cancellationToken = default)
            where TArg : class, IReturn<TResult>
            where TResult : class, IHaveRequestStatus
        {
            Initialize();
        
            var commandExecutor = ServiceProvider.Locate<IRequestHandler>();
            var executorResult = await commandExecutor.ExecuteAsync<TArg, TResult>(arg, cancellationToken).ConfigureAwait(false);
            return executorResult;
        }
        private static bool IsGenerator(Type t)
        {
            var q = from i in t.GetInterfaces()
                    where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleRequest<,>)
                    select i;

            var result = q.Any();
            return result;
        }

        private static void PopulateRegistry(Type[] types)
        {
            var commandInputsLookup =
            (from t in types
             let att = t.GetCustomAttribute<ServiceRequestAttribute>(false)
             where att is not null
             where !IsGenerator(t)
             select new { t, att.CommandType })
             .Distinct()
             .GroupBy(x => x.t)
             .ToDictionary(x => x.Key, x => x.ToArray());


            // TODO: check for errors

            var commandInputs = commandInputsLookup.ToDictionary(x => x.Key, x => x.Value[0].CommandType);



            var commands = // input, output, command (note: should be one)
            (from commandType in types
             from i in commandType.GetInterfaces()
             where i.IsGenericType
             let igtd = i.GetGenericTypeDefinition()
             where igtd == typeof(IHandleRequest<,>)
             let inputType = i.GetGenericArguments()[0]
             let outputType = i.GetGenericArguments()[1]
             select new { inputType, outputType, commandType })
                .GroupBy(x => x.inputType)
                .ToDictionary(
                    x => x.Key,
                    x => x.GroupBy(y => y.outputType)
                        .ToDictionary(
                            y => y.Key,
                            y => y.Select(z => z.commandType).ToArray()));

            var commandHandlers =
                (from t in types
                 from i in t.GetInterfaces()
                 where i.IsGenericType
                 let igtd = i.GetGenericTypeDefinition()
                 where igtd == typeof(IHandleRequest<,>)
                 let inputType = i.GetGenericArguments()[0]
                 let outputType = i.GetGenericArguments()[1]
                 select new { inputType, outputType, t })

                 .ToArray();

            var groups = commandHandlers
                .Distinct()
                .GroupBy(x => (x.inputType, x.outputType), x => x.t);

            ConventionEnforcer.AssertConventions(commandInputs, commands);

            RegisterCommands(groups);
        }

        private static void RegisterCommands(IEnumerable<IGrouping<(Type inputType, Type outputType), Type>> groups)
        {
            foreach (var handlerGroup in groups)
            {
                var inputType = handlerGroup.Key.inputType;
                var outputType = handlerGroup.Key.outputType;

                CommandHandlerTypeRegistry.Add(inputType, outputType, handlerGroup.First());
            }
        }
    }
}