﻿// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Reflection;

    internal static class ConventionEnforcer
    {
        private static bool IsGenerator(Type t)
        {
            var q = from i in t.GetInterfaces()
                    where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleRequest<,>)
                    select i;

            var result = q.Any();
            return result;
        }


        internal static void AssertConventions()
        {
            var commandInputsLookup =
            (from t in TypeRegistry.Classes
             let att = t.GetCustomAttribute<ServiceRequestAttribute>(false)
             where att is not null
             where !IsGenerator(t)
             select new { t, att.CommandType })
             .Distinct()
             .GroupBy(x => x.t)
             .ToDictionary(x => x.Key, x => x.ToArray());

            var commandInputs = commandInputsLookup.ToDictionary(x => x.Key, x => x.Value[0].CommandType);

            var commands = // input, output, command (note: should be one)
            (from commandType in TypeRegistry.Classes
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

            var errors =
                 GetMissingAttributes(commandInputs)
                 //.Union(GetCommandHandlersWithoutAttributeErrors(commands), StringComparer.OrdinalIgnoreCase)
                 .Union(GetCommandArgsWithoutAttributeErrors(commands), StringComparer.OrdinalIgnoreCase)
                 //.Union(GetCommandHandlerScopeMismatches(commands), StringComparer.OrdinalIgnoreCase)
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
            let att = command.GetCustomAttribute<ServiceRequestAttribute>(false)
            let bypass = command.GetCustomAttribute<BypassSafetyChecksAttribute>(false)
            where att is null && bypass is not null
            select $@"
The type {command.Name} is used as input for
{string.Join(", ", commands[command].Values.SelectMany(x => x.Select(z => $"{z.Name}.ExecuteAsync({command.Name}){{}}")).ToArray())}.

This requires the command to specify the 
{typeof(ServiceRequestAttribute)}, which must be 
{ServiceType.Orchestration}, {ServiceType.Calculation}, {ServiceType.Integration}, or {ServiceType.Tool}.

expecting:

[{typeof(ServiceType)}()]
class {command.Name}{{...}}
";

        private static IEnumerable<string> GetCommandHandlerScopeMismatches(Dictionary<Type, Dictionary<Type, Type[]>> commands) =>
            from x in commands
            let commandAtt = x.Key.GetCustomAttribute<ServiceRequestAttribute>()
            let commandType = commandAtt?.CommandType ?? ServiceType.None
            from y in x.Value
            from z in y.Value
            let executorAtt = z.GetCustomAttribute<ServiceRequestAttribute>()
            let executorType = executorAtt?.CommandType ?? ServiceType.None
            where commandType != executorType
            select $@"
Command scope mismatch: 
{x.Key.Name} is a command for {commandType} but used in 
{z.Name} which is an executor for {executorType}.

The command and executors must be for the same scope.

Expecting: 

[{typeof(ServiceRequestAttribute)}({nameof(ServiceRequestAttribute.CommandType)}.{commandType})]
class {z.Name}{{...}}

or...

[{typeof(ServiceRequestAttribute)}({nameof(ServiceRequestAttribute.CommandType)}.{executorType})]
class {x.Key.Name}{{...}}

";

        private static IEnumerable<string> GetCommandHandlersWithoutAttributeErrors(Dictionary<Type, Dictionary<Type, Type[]>> commands) =>
            from x in commands
            from y in x.Value
            from z in y.Value
            let att = z.GetCustomAttribute<ServiceRequestAttribute>()
            let att2 = z.GetCustomAttribute<BypassSafetyChecksAttribute>()
            where att is null && att2 is null
            select $@"
The type {z.Name} is a command executor.

This requires the class to specify the 
{typeof(ServiceRequestAttribute)}, which must be 
{ServiceType.Orchestration}, {ServiceType.Calculation}, {ServiceType.Integration}, or {ServiceType.Tool}.

expecting:

[{typeof(ServiceRequestAttribute)}()]
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
            let att = t.GetCustomAttribute<ServiceRequestAttribute>()
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
Error: the type has a [{nameof(ServiceRequestAttribute)}({typeof(ServiceType)})] attribute, 
but is not defined in any command executor.

Either remove {keyType} or create a {typeof(IHandleRequest<,>).Name} 
that takes a command of type {keyType}.
";

    }
}