// Copyright (c) TruthShield, LLC. All rights reserved.
using Microsoft.Extensions.DependencyInjection;

namespace Applinate
{
    public static class ServiceProvider
    {
        public static IServiceProvider Provider =>
            ServiceCollection.BuildServiceProvider();

        public static IServiceCollection ServiceCollection { get; private set; } = new ServiceCollection(); // UNDONE: not thread safe

        /// <summary>
        /// Locates the specified service.
        /// </summary>
        /// <typeparam name="TService">The type of the t service.</typeparam>
        /// <param name="fallback">The fallback value if no service is provided.</param>
        /// <returns>TService.</returns>
        /// <exception cref="System.InvalidOperationException">Unable to build the service provider.</exception>
        /// <exception cref="System.InvalidOperationException">no service has been registered for type {typeof(TService)}, add this by calling ServiceLocator.Register<TService,TImplementation>()")</exception>
        public static TService Locate<TService>(TService? fallback = default)
        {
            var result = Provider.GetService<TService>();

            if (result is null)
            {
                if (fallback is null)
                {
                    throw ExceptionFactory.NoRegisteredService<TService>();
                }

                return fallback;
            }

            return result;
        }

        public static void RegisterSingleton<TService, TImplementation>()
            where TService : class where TImplementation : class, TService =>
            ServiceCollection.AddSingleton<TService, TImplementation>();

        public static void RegisterSingleton<TService>(Func<IServiceProvider, TService> implementationFactory)
            where TService : class =>
            ServiceCollection.AddSingleton<TService>(implementationFactory);

        public static void RegisterSingleton<TService, TImplementation>(TImplementation implementation)
            where TService : class where TImplementation : class, TService =>
            ServiceCollection.AddSingleton<TService, TImplementation>(s => implementation);

        public static void RegisterTransient<TService>(Func<IServiceProvider, TService> implementationFactory)
            where TService : class =>
            ServiceCollection.AddTransient<TService>(implementationFactory);

        public static void RegisterTransient<TService, TImplementation>()
            where TService : class where TImplementation : class, TService, new() =>
            RegisterTransient<TService>(svc => new TImplementation());

        public static void SetServiceCollection(IServiceCollection services) =>
            ServiceCollection = services;
    }
}