// Copyright (c) TruthShield, LLC. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

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

                // TDOO: remove internals visible to and factor the code to move the appropriate functionality to the right places (the code below is too tightly coupled to Applinate internals)
                Applinate.RequestContext.Current = Applinate.RequestContext.Current with { ServiceType = ServiceType.Orchestration };

                var services = ServiceProvider.ServiceCollection;
                ServiceProvider.RegisterSingleton<IRequestHandler, DefaultRequestExecutor>();

                ExecuteFoundInitializers(services, testing);

                WireUpEventListeners();

                ServiceProvider.RegisterSingleton<IRequestHandler, DefaultRequestExecutor>();

                initialized = true;
                
            }
            finally
            {
                Applinate.RequestContext.Current = Applinate.RequestContext.Current with { ServiceType = ServiceType.None };
            }
        }

        private static void ExecuteFoundInitializers(IServiceCollection services, bool testing)
        {
            var q = from x in TypeRegistry.GetTypes()
                    where x.IsClass && x.IsAssignableTo(typeof(IInitialize))
                    let ordinal = x.GetCustomAttribute<InitializationPriorityAttribute>()?.Ordinal ?? int.MaxValue
                    orderby ordinal 
                    select x;

            var initializers = q.ToArray();

            foreach (var initializer in initializers)
            {
                var instance = Activator.CreateInstance(initializer);

                var methodInfo = typeof(IInitialize).GetMethod(nameof(IInitialize.Initialize));

                methodInfo?.Invoke(instance, new object[] { services, testing });
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