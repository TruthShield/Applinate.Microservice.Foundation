// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    internal sealed class EmptyInstanceRegistry : IInstanceRegistry
    {
        private static Exception BuildException() => new InvalidOperationException("No Inversion Of Control implementation has been registered"); // TODO: more

        object? IInstanceRegistry.GetInstance(Type serviceType) => throw BuildException();

        void IInstanceRegistry.RegisterSingleton<TAbstraction>(Func<TAbstraction> factory) => throw BuildException();

        void IInstanceRegistry.RegisterTransient<TAbstraction>(Func<TAbstraction> factory) => throw BuildException();
    }
}