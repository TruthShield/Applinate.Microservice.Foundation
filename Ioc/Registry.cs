// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class Registry : IInstanceRegistry
    {
        private static IServiceProvider Provider => ServiceCollection.BuildServiceProvider();

        private static IServiceCollection ServiceCollection { get; } = new ServiceCollection();

        object? IInstanceRegistry.Get(Type serviceType) => Provider.GetService(serviceType);

        void IInstanceRegistry.Register<TAbstraction>(
            Func<TAbstraction> factory, 
            InstanceLifetime lifetime = InstanceLifetime.Transient)
        {
            switch(lifetime)
            {
                case InstanceLifetime.Undefined:
                    throw new InvalidOperationException($"The {nameof(InstanceLifetime)} is {nameof(InstanceLifetime.Undefined)}.  You must defin the instance lifetime.");
                case InstanceLifetime.Singleton:
                    ServiceCollection.AddSingleton(sp => factory());
                    break;
                case InstanceLifetime.Transient:
                    ServiceCollection.AddTransient(sp => factory());
                    break;
                default:
                    throw new NotSupportedException($"{nameof(InstanceLifetime)}.{nameof(InstanceLifetime.Undefined)} is not supported");

            }
        }
    }
}