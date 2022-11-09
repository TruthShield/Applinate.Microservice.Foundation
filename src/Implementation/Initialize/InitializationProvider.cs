// Copyright (c) TruthShield, LLC. All rights reserved.

using System.Diagnostics;

namespace Applinate
{    
    public static class InitializationProvider
    {
        private static bool initialized = false;
        private static object initializedLock = new object();

        [STAThread]
        public static void Initialize(bool testing = false)
        {
            try
            {
                if (initialized)
                {
                    return;
                }

                // TDOO: look into removing internals visible to and factor the code to move the appropriate functionality to the right places (the code below is too tightly coupled to Applinate internals)
                Applinate.RequestContext.Current = Applinate.RequestContext.Current with { ServiceType = ServiceType.Orchestration };

                RegisterServiceFactories();

                ExecuteInitializers(testing);

                WireUpEventListeners();

                ServiceProvider.RegisterInstance<IRequestExecutor, RequestExecutor>(InstanceLifetime.Singleton);

                initialized = true;
                
            }
            finally
            {
                Applinate.RequestContext.Current = Applinate.RequestContext.Current with { ServiceType = ServiceType.None };
            }
        }

        private static void RegisterServiceFactories()
        {
            var factories = TypeRegistry.ServiceFactories;

            if(! factories.Any())
            {
                throw new InvalidOperationException("no available service factory");
            }

            if(factories.Skip(1).Any())
            {
                throw new InvalidOperationException("too many service factories");
            }

            InstanceRegistry.Instance = Activator.CreateInstance(factories.First()) as IInstanceRegistry ?? throw new InvalidOperationException("not a IServiceFactory"); // note: should not throw
        }

        private static void ExecuteInitializers( bool testing)
        {
            foreach (var initializer in TypeRegistry.Initializers)
            {
                var instance = Activator.CreateInstance(initializer);

                var methodInfo = typeof(IInitialize).GetMethod(nameof(IInitialize.Initialize));

                methodInfo?.Invoke(instance, new object[] { testing });
            }
        }

        private static void OnCommandContextChange(object? sender, RequestContextChange e)
        {
            string message = "CommandContext Changed " + e.ToString();
            Debug.WriteLine(message);
            Trace.TraceInformation(message);
            //Service.Locate<ILogger>()?.Log(LogLevel.Information, message); //todo: wire in logging
        }

        [Conditional("DEBUG")]
        private static void WireUpEventListeners()
        {
            InfrastructureEventSink.For.AnyContextChange().Changed += OnCommandContextChange;
        }
    }
}