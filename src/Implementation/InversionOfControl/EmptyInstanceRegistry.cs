// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    internal sealed class EmptyInstanceRegistry : IInstanceRegistry
    {
        private static Exception BuildException() => 
            new InvalidOperationException("No Inversion Of Control implementation has been registered"); // TODO: more

        object? IInstanceRegistry.Get(Type serviceType) => 
            throw BuildException();

        void IInstanceRegistry.Register<TAbstraction>(
            Func<TAbstraction> factory, 
            InstanceLifetime lifetime = InstanceLifetime.Transient) => 
            throw BuildException();

    }
}