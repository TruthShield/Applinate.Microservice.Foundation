// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Reflection;

    internal class ServiceProxyFactory:IInitialize
    {
        public bool SkipDuringTesting => false;

        private static bool _Initialized = false;

        internal static TAbstraction Build<TAbstraction>() => 
            ServiceProxy<TAbstraction>.Generate();

        internal static void Register<TAbstraction>() where TAbstraction:class =>
            ServiceProvider.RegisterSingleton<TAbstraction>(() => Build<TAbstraction>()) ;

        [STAThread]
        public void Initialize(bool testing = false)
        {
            if(_Initialized)
            {
                return;
            }

            _Initialized = true;

            var qry = from t in TypeRegistry.Types
                      where t.IsInterface
                      let st = t.GetCustomAttribute<ServiceAttribute>()
                      where st != null
                      select t;

            foreach(var serviceInterface in qry)
            {
                var mth = typeof(ServiceProxyFactory)?
                    .GetMethod(nameof(ServiceProxyFactory.Register), BindingFlags.Static | BindingFlags.NonPublic) 
                    ?? throw ExceptionFactory.UnexpectedNull();

                mth
                    .MakeGenericMethod(serviceInterface)
                    .Invoke(null, null);
            }        
        }

        private class ServiceProxy<TInterface> : DispatchProxy
        {
            private static readonly Dictionary<MethodInfo, Func<object[], object>> _Execution = new();

            private static bool _Initialzed;

            public static TInterface Generate() =>
                Create<TInterface, ServiceProxy<TInterface>>();

            public Task<TResponse> ExecuteAsync<TRequest, TResponse>(
                TRequest request,
                CancellationToken cancellationToken)
                where TRequest : class, IReturn<TResponse>
                where TResponse : class, IHaveRequestStatus =>
                request.ExecuteAsync(cancellationToken);

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
                if (!_Initialzed)
                {
                    Initialize();
                }

                return _Execution[targetMethod](args);
            }

            private object? Execute(Type[] types, object[] args) =>
                GetType()?
                .GetMethod(nameof(ExecuteAsync))?
                .MakeGenericMethod(types)
                .Invoke(this, args) ?? throw ExceptionFactory.UnexpectedNull();

            [STAThread]
            private void Initialize()
            {
                if (_Initialzed)
                {
                    return;
                }

                var serviceAttribute = typeof(TInterface).GetCustomAttribute<ServiceAttribute>();

                if (serviceAttribute is null)
                {
                    throw new InvalidOperationException("service type is not correct");
                }

                var serviceCommandType = serviceAttribute.CommandType;

                var methods = typeof(TInterface).GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    RegisterMethod(method);
                }

                _Initialized = true;
            }

            private void RegisterMethod(MethodInfo method)
            {
                var returnType = method.ReturnType;
                var parameterInfos = method.GetParameters();

                if (parameterInfos.Length < 1)
                {
                    throw new InvalidOperationException("invalid - 1-2 parameters");
                }

                if (parameterInfos.Length > 2)
                {
                    throw new InvalidOperationException("invlid - 1-2 parameters");
                }

                if (parameterInfos.Length == 2)
                {
                    if (parameterInfos[1].ParameterType != typeof(CancellationToken))
                    {
                        throw new InvalidOperationException("2nd param should be calcellation token");
                    }
                }

                var first = parameterInfos[0].ParameterType;

                if (returnType.GetGenericTypeDefinition() != typeof(Task<>))
                {
                    throw new InvalidOperationException("bad 0");
                }

                var returnInnerType = returnType.GenericTypeArguments[0];

                if (!first.IsClass)
                {
                    throw new InvalidOperationException("bad 1");
                }

                if (!returnInnerType.IsAssignableTo(typeof(IHaveRequestStatus)))
                {
                    throw new InvalidOperationException("bad 2");
                }

                if (!returnInnerType.IsClass)
                {
                    throw new InvalidOperationException("bad 3");
                }

                // var taskType = typeof(Task<>).MakeGenericType(returnInnerType);
                var targetParamInterface = typeof(IReturn<>).MakeGenericType(returnInnerType);

                if (!first.GetInterfaces().Any(x => x.IsAssignableTo(targetParamInterface)))
                {
                    throw new InvalidOperationException("bad 4");
                }

                _Execution.Add(method, new((args) => Execute(new[] { first, returnInnerType }, args) ?? throw ExceptionFactory.UnexpectedNull()));
            }
        }
    }
}