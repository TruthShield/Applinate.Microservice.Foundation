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
        private static readonly IDictionary<(Type, Type), Type> CommandHandlerTypeRegistry = new Dictionary<(Type, Type), Type>();

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
            var key = (typeof(TArg), typeof(TResult));

            if (!CommandHandlerTypeRegistry.ContainsKey(key))
            {
                // fault on execution because the behavior may be overriden by an interceptor
                return new FaultGeneratingCommandExecutor<TArg, TResult>(() =>
                    ExceptionFactory.NoDefinedService<TArg, TResult>());
            }

            var type = CommandHandlerTypeRegistry[key];

            var instance = Activator.CreateInstance(type) as IHandleRequest<TArg, TResult>;

            return instance ?? throw ExceptionFactory.NoDefinedService<TArg, TResult>();
        }

        private static void AssertConventions(
            Dictionary<Type, ServiceType> commandInputs,
            Dictionary<Type, Dictionary<Type, Type[]>> commands)
        {
            var errors =
                 GetMissingAttributes(commandInputs)
                 .Union(GetCommandHandlersWithoutAttributeErrors(commands), StringComparer.OrdinalIgnoreCase)
                 .Union(GetCommandArgsWithoutAttributeErrors(commands), StringComparer.OrdinalIgnoreCase)
                 .Union(GetCommandHandlerScopeMismatches(commands), StringComparer.OrdinalIgnoreCase)
                 .Union(GetDupeCommandErrors(commands), StringComparer.OrdinalIgnoreCase)
                 .ToArray();

            if (errors.Length == 0)
            {
                return;
            }

            var errorMessage = "Errors were found with the type definitions in the solution: \n\n" + string.Join("\n\n* ", errors);

            throw new InvalidOperationException(errorMessage);
        }

        private static IEnumerable<string> GetCommandArgsWithoutAttributeErrors(Dictionary<Type, Dictionary<Type, Type[]>> commands) =>
            from command in commands.Keys
            let att = command.GetCustomAttribute<ServiceAttribute>(false)
            let bypass = command.GetCustomAttribute<BypassSafetyChecksAttribute>(false)
            where att is null && bypass is not null
            select $@"
The type {command.Name} is used as input for
{string.Join(", ", commands[command].Values.SelectMany(x => x.Select(z => $"{z.Name}.ExecuteAsync({command.Name}){{}}")).ToArray())}.

This requires the command to specify the 
{typeof(ServiceAttribute)}, which must be 
{ServiceType.Orchestration}, {ServiceType.Calculation}, {ServiceType.Integration}, or {ServiceType.Tool}.

expecting:

[{typeof(ServiceType)}()]
class {command.Name}{{...}}
";

        private static IEnumerable<string> GetCommandHandlerScopeMismatches(Dictionary<Type, Dictionary<Type, Type[]>> commands) =>
            from x in commands
            let commandAtt = x.Key.GetCustomAttribute<ServiceAttribute>()
            let commandType = commandAtt?.CommandType ?? ServiceType.None
            from y in x.Value
            from z in y.Value
            let executorAtt = z.GetCustomAttribute<ServiceAttribute>()
            let executorType = executorAtt?.CommandType ?? ServiceType.None
            where commandType != executorType
            select $@"
Command scope mismatch: 
{x.Key.Name} is a command for {commandType} but used in 
{z.Name} which is an executor for {executorType}.

The command and executors must be for the same scope.

Expecting: 

[{typeof(ServiceAttribute)}({nameof(ServiceAttribute.CommandType)}.{commandType})]
class {z.Name}{{...}}

or...

[{typeof(ServiceAttribute)}({nameof(ServiceAttribute.CommandType)}.{executorType})]
class {x.Key.Name}{{...}}

";

        private static IEnumerable<string> GetCommandHandlersWithoutAttributeErrors(Dictionary<Type, Dictionary<Type, Type[]>> commands) =>
            from x in commands
            from y in x.Value
            from z in y.Value
            let att = z.GetCustomAttribute<ServiceAttribute>()
            let att2 = z.GetCustomAttribute<BypassSafetyChecksAttribute>()
            where att is null && att2 is null
            select $@"
The type {z.Name} is a command executor.

This requires the class to specify the 
{typeof(ServiceAttribute)}, which must be 
{ServiceType.Orchestration}, {ServiceType.Calculation}, {ServiceType.Integration}, or {ServiceType.Tool}.

expecting:

[{typeof(ServiceAttribute)}()]
class {z.Name}{{...}}

";

        private static IEnumerable<string> GetDupeCommandErrors(Dictionary<Type, Dictionary<Type, Type[]>> commandsByType) =>
            from x in commandsByType
            let inputType = x.Key
            from y in commandsByType[inputType]
            let outputType = y.Key
            let executors = GetExecutors(y.Value).ToArray()
            where executors.Skip(1).Any()
            from executor in executors
            select $@"
The type {executor.Name} has a duplicate input/output pair with 
{string.Join(" ", executors.Select(x => x.Name).ToArray())}.

The system can not determine the correct executor to use. There
can only be one executor defined for every {inputType.Name}.

Remove {executor.Name} or the other duplicate command definition.
";

        private static IEnumerable<Type> GetExecutors(Type[] types) =>
            from t in types
            let att = t.GetCustomAttribute<ServiceAttribute>()
            where att != null && att.CommandType != ServiceType.None
            select t;

        private static IEnumerable<string> GetMissingAttributes(Dictionary<Type, ServiceType> commandInputs) => 
            from x in commandInputs
            where x.Value == ServiceType.None
            select @$"
The type {x.Key} is missing the {typeof(ServiceType)} attribute, 
which must be {ServiceType.Orchestration}, {ServiceType.Calculation}, {ServiceType.Integration}, or {ServiceType.Tool}

expecting:

[{typeof(ServiceType)}()]
class {x.Key}{{...}}";

        private static IEnumerable<string> GetOrphanedInputErrors(Dictionary<Type, ServiceType> commandInputs, Dictionary<Type, Dictionary<Type, Type[]>> commands) =>
            from x in commandInputs
            let keyType = x.Key
            where !commands.ContainsKey(keyType)
            select $@"
Type: {keyType} 
Assembly: '{keyType.Assembly.GetName().Name}' 
Error: the type has a [{nameof(ServiceAttribute)}({typeof(ServiceType)})] attribute, 
but is not defined in any command executor.

Either remove {keyType} or create a {typeof(IHandleRequest<,>).Name} 
that takes a command of type {keyType}.
";

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

                var q = TypeRegistry.GetTypes().Where(x => x.IsClass).ToArray();

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
             let att = t.GetCustomAttribute<ServiceAttribute>(false)
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

            AssertConventions(commandInputs, commands);

            RegisterCommands(groups);
        }

        private static void RegisterCommands(IEnumerable<IGrouping<(Type inputType, Type outputType), Type>> groups)
        {
            foreach (var handlerGroup in groups)
            {
                var inputType = handlerGroup.Key.inputType;
                var outputType = handlerGroup.Key.outputType;

                CommandHandlerTypeRegistry.Add((inputType, outputType), handlerGroup.First());
            }
        }
    }
}