// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public static class ServiceProvider
    {
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

            return result is null ? fallback is null ? throw ExceptionFactory.NoRegisteredService<TService>() : fallback : result;
        }

        public static void Register<TAbstraction>(
            Func<TAbstraction> factory, 
            InstanceLifetime lifetime = InstanceLifetime.Transient)
            where TAbstraction : class =>
            InstanceRegistry.RegisterInstance(factory, lifetime);

        internal static void Register<TAbstraction, TConcretion>(
            InstanceLifetime lifetime = InstanceLifetime.Transient)
            where TConcretion : TAbstraction, new()
            where TAbstraction: class =>
            Register<TAbstraction>(() => new TConcretion(), lifetime);
    }
}