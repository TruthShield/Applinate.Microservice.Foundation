// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    internal sealed class EmptyInstanceRegistry : IInstanceRegistry
    {
        private static Exception BuildException() => 
            new InvalidOperationException(@$"
No Inversion Of Control implementation has been registered.

To fix this, you have to provide a class that implements {nameof(IInstanceRegistry)}
that maps to the IoC (Inversion of Control) framework you would like to use.

You can create your own or use a NuGet package like 
Applinate.Microservice.InversionOfControl.Microsoft
");

        object? IInstanceRegistry.Get(Type serviceType) => 
            throw BuildException();

        void IInstanceRegistry.Register<TAbstraction>(
            Func<TAbstraction> factory, 
            InstanceLifetime lifetime) => 
            throw BuildException();

    }
}