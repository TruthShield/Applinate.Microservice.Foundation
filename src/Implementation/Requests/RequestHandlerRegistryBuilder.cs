// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class RequestHandlerRegistryBuilder
    {
        internal static NestedDictionary<Type, Type, IRequestHandlerBuilder> BuildRegistry()
        {
            

            // TODO: check for errors

           

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

            ConventionEnforcer.AssertConventions();

            return RegisterMessageCommands(groups);
        }

  
        private static NestedDictionary<Type, Type, IRequestHandlerBuilder> RegisterMessageCommands(IEnumerable<IGrouping<(Type inputType, Type outputType), Type>> groups)
        {
            var result = new NestedDictionary<Type, Type, IRequestHandlerBuilder>();

            foreach (var handlerGroup in groups)
            {
                var inputType = handlerGroup.Key.inputType;
                var outputType = handlerGroup.Key.outputType;

                result.Add(inputType, outputType, new MessageRequestHandlerBuilder(inputType, outputType, handlerGroup.First()));
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

            public IHandleRequest<TArg1, TResult1> BuildRequestHandler<TArg1, TResult1>()
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

        private class ServiceRequestHandlerMap<TRequest, TResponse> : IHandleRequest<TRequest, TResponse>
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

        private class ServiceRequestHandlerBuilder : IRequestHandlerBuilder
        {
            public ServiceRequestHandlerBuilder(Type implementationType, MethodInfo methodInfo)
            {
                ImplementationType = implementationType;
                MethodInfo = methodInfo;
            }

            public Type ImplementationType { get; }
            public MethodInfo MethodInfo { get; }

            public IHandleRequest<TArg1, TResult1> BuildRequestHandler<TArg1, TResult1>()
                where TArg1 : class, IReturn<TResult1>
                where TResult1 : class, IHaveRequestStatus => 
                new ServiceRequestHandlerMap<TArg1, TResult1>(ImplementationType, MethodInfo);
        }
    }
}