// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Reflection;

    internal static class RequestHandlerRegistryBuilder
    {
        internal static NestedDictionary<Type, Type, IRequestHandlerBuilder[]> BuildHandlerRegistry()
        {
            ConventionEnforcer.AssertConventions();

            var messageHandlers = BuildMessageCommanHandlers();
            var serviceHandlers = BuildserviceCommandHandlers();
            var allHandlers = Add(messageHandlers, serviceHandlers);

            return allHandlers;
        }

        private static NestedDictionary<Type, Type, IRequestHandlerBuilder[]> Add(NestedDictionary<Type, Type, IRequestHandlerBuilder[]> messageHandlers, NestedDictionary<Type, Type, IRequestHandlerBuilder[]> serviceHandlers)
        {
            var x = new NestedDictionary<Type, Type, IRequestHandlerBuilder[]>(messageHandlers);

            foreach (var outer in serviceHandlers)
            {
                var key1 = outer.Key;

                foreach (var inner in outer.Value)
                {
                    var key2 = inner.Key;

                    var values = inner.Value;

                    if (x.ContainsKey(key1, key2))
                    {
                        x[key1][key2] = x[key1][key2].Union(values).ToArray();
                    }
                    else
                    {
                        x.Add(key1, key2, values);
                    }
                }
            }

            return x;
        }

        private static NestedDictionary<Type, Type, IRequestHandlerBuilder[]> BuildMessageCommanHandlers()
        {
            var commandHandlers =
                (from t in TypeRegistry.Classes
                 from i in t.GetInterfaces()
                 where i.IsGenericType
                 let igtd = i.GetGenericTypeDefinition()
                 where igtd == typeof(IRequestHandler<,>)
                 let inputType = i.GetGenericArguments()[0]
                 let outputType = i.GetGenericArguments()[1]
                 select new { inputType, outputType, t })

                 .ToArray();

            var groups = commandHandlers
                .Distinct()
                .GroupBy(x => (x.inputType, x.outputType), x => x.t);

            var result = new NestedDictionary<Type, Type, IRequestHandlerBuilder[]>();

            foreach (var handlerGroup in groups)
            {
                var inputType = handlerGroup.Key.inputType;
                var outputType = handlerGroup.Key.outputType;

                var handlers = handlerGroup.Select(x => new MessageRequestHandlerBuilder(inputType, outputType, x)).ToArray();

                result.Add(inputType, outputType, handlers);
            }

            return result;
        }

        private static NestedDictionary<Type, Type, IRequestHandlerBuilder[]> BuildserviceCommandHandlers()
        {
            var qry =
                from c in TypeRegistry.Classes
                from i in c.GetInterfaces()
                let si = i.GetCustomAttribute<ServiceAttribute>()
                where si is not null
                from m in i.GetMethods()
                let args = m.GetParameters()
                where args.Length == 1 ||
                   (args.Length == 2 && args[1].ParameterType == typeof(CancellationToken))
                let ret = m.ReturnParameter.ParameterType
                where ret.GetGenericTypeDefinition() == typeof(Task<>)
                let innerRet = ret.GetGenericArguments()[0]
                let a0 = args[0].ParameterType
                let targetType = typeof(IReturn<>).MakeGenericType(innerRet)
                where a0.IsAssignableTo(targetType)
                select (innerRet, args[0].ParameterType, m, c, i);

            var items = qry.GroupBy(x => (x.ParameterType, x.innerRet)).ToArray();

            var result = new NestedDictionary<Type, Type, IRequestHandlerBuilder[]>();

            foreach (var item in items)
            {
                var key1 = item.Key.ParameterType;
                var key2 = item.Key.innerRet;

                result.Add(key1, key2,
                    item.Select(x => new ServiceRequestHandlerBuilder(x.c, x.m) as IRequestHandlerBuilder).ToArray());
            }

            return result;
        }

        private class MessageRequestHandlerBuilder : IRequestHandlerBuilder
        {
            public MessageRequestHandlerBuilder(Type argType, Type resultType, Type implementationType)
            {
                ArgType = argType;
                ResultType = resultType;
                ImplementationType = implementationType;
            }

            public Type ArgType { get; }
            public Type ImplementationType { get; }
            public Type ResultType { get; }

            public IRequestHandler<TRequest1, TResult1> BuildRequestHandler<TRequest1, TResult1>()
                where TRequest1 : class, IReturn<TResult1>
                where TResult1 : class, IHaveRequestStatus
            {
                if (typeof(TRequest1) != ArgType)
                {
                    throw new ArgumentException("expecting type " + ArgType);
                }

                if (typeof(TResult1) != ResultType)
                {
                    throw new ArgumentException("expecting type " + ResultType);
                }

                return Activator.CreateInstance(ImplementationType) as IRequestHandler<TRequest1, TResult1> ?? throw ExceptionFactory.UnexpectedNull();
            }
        }

        private class ServiceRequestHandlerBuilder : IRequestHandlerBuilder
        {
            public ServiceRequestHandlerBuilder(Type implementationType, MethodInfo methodInfo)
            {
                ImplementationType = implementationType;
                MethodInfo = methodInfo;
            }

            public Type ImplementationType { get; }
            public MethodInfo MethodInfo { get; }

            public IRequestHandler<TRequest1, TResult1> BuildRequestHandler<TRequest1, TResult1>()
                where TRequest1 : class, IReturn<TResult1>
                where TResult1 : class, IHaveRequestStatus =>
                new ServiceRequestHandlerMap<TRequest1, TResult1>(ImplementationType, MethodInfo);
        }

        private class ServiceRequestHandlerMap<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
                    where TRequest : class, IReturn<TResponse>
            where TResponse : class, IHaveRequestStatus
        {
            public ServiceRequestHandlerMap(Type implementationType, MethodInfo methodInfo)
            {
                ImplementationType = implementationType;
                MethodInfo = methodInfo;
            }

            public Type ImplementationType { get; }
            public MethodInfo MethodInfo { get; }

            public Task<TResponse> ExecuteAsync(TRequest arg, CancellationToken cancellationToken = default) =>
                MethodInfo.Invoke(
                    Activator.CreateInstance(ImplementationType),
                    new object[]
                    {
                        arg,
                        cancellationToken
                    }) as Task<TResponse> ?? throw new InvalidCastException();
        }
    }
}