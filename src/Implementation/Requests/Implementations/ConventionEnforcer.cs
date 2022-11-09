// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Reflection;

    internal static class ConventionEnforcer
    {
        private static bool IsGenerator(Type t)
        {
            var q = from i in t.GetInterfaces()
                    where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                    select i;

            var result = q.Any();
            return result;
        }

        internal static void AssertConventions()
        {
            var requestParamLookup =
            (from t in TypeRegistry.Classes
             let att = t.GetCustomAttribute<ServiceRequestAttribute>(false)
             where att is not null
             where !IsGenerator(t)
             select new { t, att.ServiceType })
             .Distinct()
             .GroupBy(x => x.t)
             .ToDictionary(x => x.Key, x => x.ToArray());

            var requestParams = requestParamLookup.ToDictionary(x => x.Key, x => x.Value[0].ServiceType);

            var requests = // input, output, request (note: should be one)
            (from t in TypeRegistry.Classes
             from i in t.GetInterfaces()
             where i.IsGenericType
             let igtd = i.GetGenericTypeDefinition()
             where igtd == typeof(IRequestHandler<,>)
             let requestType = i.GetGenericArguments()[0]
             let responseType = i.GetGenericArguments()[1]
             select new { requestType, responseType, t})
                .GroupBy(x => x.requestType)
                .ToDictionary(
                    x => x.Key,
                    x => x.GroupBy(y => y.responseType)
                        .ToDictionary(
                            y => y.Key,
                            y => y.Select(z => z.t).ToArray()));

            var errors =
                 GetMissingAttributes(requestParams)
                 .Union(GetRequestsWithoutAttributeErrors(requests), StringComparer.OrdinalIgnoreCase)
                 .Union(GetDupeRequestErrors(requests), StringComparer.OrdinalIgnoreCase)
                 .ToArray();

            if (errors.Length == 0)
            {
                return;
            }

            var errorMessage = "Errors were found with the type definitions in the solution: \n\n" + string.Join("\n\n* ", errors);

            throw new InvalidOperationException(errorMessage);
        }

        private static IEnumerable<string> GetRequestsWithoutAttributeErrors(Dictionary<Type, Dictionary<Type, Type[]>> requests) =>
            from request in requests.Keys
            let att = request.GetCustomAttribute<ServiceRequestAttribute>(false)
            let bypass = request.GetCustomAttribute<BypassSafetyChecksAttribute>(false)
            where att is null && bypass is not null
            select $@"
The type {request.Name} is used as input for
{string.Join(", ", requests[request].Values.SelectMany(x => x.Select(z => $"{z.Name}.ExecuteAsync({request.Name}){{}}")).ToArray())}.

This requires the request to specify the 
{typeof(ServiceRequestAttribute)}, which must be 
{ServiceType.Orchestration}, {ServiceType.Calculation}, {ServiceType.Integration}, or {ServiceType.Tool}.

expecting:

[{typeof(ServiceType)}()]
class {request.Name}{{...}}
";

        private static IEnumerable<string> GetCommandHandlerScopeMismatches(Dictionary<Type, Dictionary<Type, Type[]>> requests) =>
            from x in requests
            let serviceRequestAttribute = x.Key.GetCustomAttribute<ServiceRequestAttribute>()
            let requestType = serviceRequestAttribute?.ServiceType ?? ServiceType.None
            from y in x.Value
            from z in y.Value
            let executorAtt = z.GetCustomAttribute<ServiceRequestAttribute>()
            let executorType = executorAtt?.ServiceType ?? ServiceType.None
            where requestType != executorType
            select $@"
Command scope mismatch: 
{x.Key.Name} is a command for {requestType} but used in 
{z.Name} which is an executor for {executorType}.

The command and executors must be for the same scope.

Expecting: 

[{typeof(ServiceRequestAttribute)}({nameof(ServiceRequestAttribute.ServiceType)}.{requestType})]
class {z.Name}{{...}}

or...

[{typeof(ServiceRequestAttribute)}({nameof(ServiceRequestAttribute.ServiceType)}.{executorType})]
class {x.Key.Name}{{...}}

";

        private static IEnumerable<string> GetDupeRequestErrors(Dictionary<Type, Dictionary<Type, Type[]>> requestsByType) =>
            from x in requestsByType
            let inputType = x.Key
            from y in requestsByType[inputType]
            let responseType = y.Key
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
            where att != null && att.ServiceType != ServiceType.None
            select t;

        private static IEnumerable<string> GetMissingAttributes(Dictionary<Type, ServiceType> requests) =>
            from x in requests
            where x.Value == ServiceType.None
            select @$"
The type {x.Key} is missing the {typeof(ServiceType)} attribute, 
which must be {ServiceType.Orchestration}, {ServiceType.Calculation}, {ServiceType.Integration}, or {ServiceType.Tool}

expecting:

[{typeof(ServiceType)}()]
class {x.Key}{{...}}";

        private static IEnumerable<string> GetOrphanedInputErrors(Dictionary<Type, ServiceType> requests, Dictionary<Type, Dictionary<Type, Type[]>> commands) =>
            from x in requests
            let keyType = x.Key
            where !commands.ContainsKey(keyType)
            select $@"
Type: {keyType} 
Assembly: '{keyType.Assembly.GetName().Name}' 
Error: the type has a [{nameof(ServiceRequestAttribute)}({typeof(ServiceType)})] attribute, 
but is not defined in any request executor.

Either remove {keyType} or create a {typeof(IRequestHandler<,>).Name} 
that takes a request of type {keyType}.
";

    }
}