// Copyright (c) TruthShield, LLC. All rights reserved.

using System.Diagnostics;

namespace Applinate
{    
    public static class InitializationProvider
    {
        private static bool initialized = false;

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
                RequestContextProvider.Instance = RequestContextProvider.Instance with { ServiceType = ServiceType.Orchestration };

                RegisterServiceFactories();

                ExecuteInitializers(testing);

                WireUpEventListeners();

                ServiceProvider.Register<IRequestExecutor, RequestExecutor>(InstanceLifetime.Singleton);

                initialized = true;
                
            }
            finally
            {
                RequestContextProvider.Instance = RequestContextProvider.Instance with { ServiceType = ServiceType.None };
            }
        }

        private static void RegisterServiceFactories()
        {
            var factories = TypeRegistry.ServiceFactories;

            if(! factories.Any())
            {
                throw new InvalidOperationException(@$"
No Inversion Of Control implementation has been registered.

To fix this, you have to provide a class that implements {nameof(IInstanceRegistry)}
that maps to the IoC (Inversion of Control) framework you would like to use.

You can create your own or use a NuGet package like 
Applinate.Microservice.InversionOfControl.Microsoft
");
            }

            if(factories.Skip(1).Any())
            {
                throw new InvalidOperationException(@$"
Too many Inversion Of Control implementation have been registered.

To fix this, you have to provide a single class that implements {nameof(IInstanceRegistry)}
that maps to the IoC (Inversion of Control) framework you would like to use.

You can create your own or use a NuGet package like 
Applinate.Microservice.InversionOfControl.Microsoft
");
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
        }

        [Conditional("DEBUG")]
        private static void WireUpEventListeners()
        {
            InfrastructureEventSink.For.AnyContextChange().Changed += OnCommandContextChange;
        }
    }
}