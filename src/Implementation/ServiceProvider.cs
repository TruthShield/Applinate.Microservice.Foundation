// Copyright (c) TruthShield, LLC. All rights reserved.
using Microsoft.Extensions.DependencyInjection;

namespace Applinate
{
    public static class ServiceProvider
    {
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
            var result = InstanceRegistry.GetInstance<TService>();

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

        public static void RegisterSingleton<TAbstraction>(Func<TAbstraction> factory)
            where TAbstraction : class =>
            InstanceRegistry.RegisterSingleton(factory);

        public static void RegisterTransient<TAbstraction>(Func<TAbstraction> factory)
            where TAbstraction : class =>
            InstanceRegistry.RegisterTransient(factory);

        public static void SetServiceCollection(IServiceCollection services) =>
            ServiceCollection = services;

        internal static void RegisterSingleton<TAbstraction, TConcretion>()
            where TConcretion : TAbstraction, new()
            where TAbstraction: class =>
            RegisterSingleton<TAbstraction>(() => new TConcretion());
    }
}