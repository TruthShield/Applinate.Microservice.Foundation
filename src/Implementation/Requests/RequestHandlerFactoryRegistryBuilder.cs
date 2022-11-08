// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Reflection;

    internal static class RequestHandlerFactoryRegistryBuilder
    {
        internal static NestedDictionary<Type, Type, IHandlerFactory> BuildRegistry()
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

            // TODO: check for errors

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

            var commandHandlers =
                (from t in TypeRegistry.Classes
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

            return RegisterMessageCommands(groups);
        }

        private static bool IsGenerator(Type t)
        {
            var q = from i in t.GetInterfaces()
                    where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleRequest<,>)
                    select i;

            var result = q.Any();
            return result;
        }

        private static NestedDictionary<Type, Type, IHandlerFactory> RegisterMessageCommands(IEnumerable<IGrouping<(Type inputType, Type outputType), Type>> groups)
        {
            var result = new NestedDictionary<Type, Type, IHandlerFactory>();
            foreach (var handlerGroup in groups)
            {
                var inputType = handlerGroup.Key.inputType;
                var outputType = handlerGroup.Key.outputType;

                result.Add(inputType, outputType, new MessageHandlerFactory(inputType, outputType, handlerGroup.First()));
            }

            return result;
        }

        private class MessageHandlerFactory : IHandlerFactory
        {
            public MessageHandlerFactory(Type argType, Type resultType, Type implementationType)
            {
                ArgType = argType;
                ResultType = resultType;
                ImplementationType = implementationType;
            }

            public Type ArgType { get; }
            public Type ImplementationType { get; }
            public Type ResultType { get; }

            public IHandleRequest<TArg1, TResult1> Build<TArg1, TResult1>()
                where TArg1 : class, IReturn<TResult1>
                where TResult1 : class, IHaveRequestStatus
            {
                if (typeof(TArg1) != ArgType)
                {
                    throw new ArgumentException("expecting type " + ArgType);
                }

                if (typeof(TResult1) != ResultType)
                {
                    throw new ArgumentException("expecting type " + ResultType);
                }

                return Activator.CreateInstance(ImplementationType) as IHandleRequest<TArg1, TResult1>;
            }
        }

        private class ServiceHandlerFactory : IHandlerFactory
        {
            IHandleRequest<TArg1, TResult1> IHandlerFactory.Build<TArg1, TResult1>()
            {
                throw new NotImplementedException();
            }
        }
    }
}