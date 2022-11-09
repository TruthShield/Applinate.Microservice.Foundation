// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public interface IInstanceRegistry
    {
        object? Get(Type serviceType);

        void Register<TAbstraction>(
            Func<TAbstraction> factory, 
            InstanceLifetime lifetime = InstanceLifetime.Transient)
            where TAbstraction : class;
    }
}