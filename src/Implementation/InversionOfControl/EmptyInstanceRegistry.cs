// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    internal sealed class EmptyInstanceRegistry : IInstanceRegistry
    {
        void IInstanceRegistry.RegisterSingleton<TService, TImplementation>() => throw BuildException();

        private static Exception BuildException() => new InvalidOperationException("No Inversion Of Control implementation has been registered"); // TODO: more

        object? IInstanceRegistry.GetInstance(Type serviceType) => throw BuildException();

        void IInstanceRegistry.RegisterSingleton<TAbstraction>(Func<TAbstraction> factory) => throw BuildException();

        void IInstanceRegistry.RegisterSingleton<TAbstraction, TConcretion>(Func<TAbstraction, TConcretion> factory) => throw BuildException();

        void IInstanceRegistry.RegisterTransient<TAbstraction>(Func<TAbstraction> factory) => throw BuildException();
    }
}