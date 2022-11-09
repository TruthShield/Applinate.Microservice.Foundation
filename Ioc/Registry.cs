// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class Registry : IInstanceRegistry
    {
        private static IServiceProvider Provider => ServiceCollection.BuildServiceProvider();

        private static IServiceCollection ServiceCollection { get; } = new ServiceCollection(); // UNDONE: not thread safe

        void IInstanceRegistry.RegisterSingleton<TAbstraction, TConcretion>() =>
            ServiceCollection.AddSingleton<TAbstraction, TConcretion>();

        void IInstanceRegistry.RegisterSingleton<TAbstraction>(Func<TAbstraction> factory) =>
            ServiceCollection.AddSingleton(sp => factory());

        void IInstanceRegistry.RegisterSingleton<TAbstraction, TConcretion>(Func<TAbstraction, TConcretion> factory) =>
            ServiceCollection.AddSingleton(factory);

        void IInstanceRegistry.RegisterTransient<TAbstraction>(Func<TAbstraction> factory) =>
            ServiceCollection.AddTransient(sp => factory());

        object? IInstanceRegistry.GetInstance(Type serviceType) => 
            Provider.GetService(serviceType);
    }
}