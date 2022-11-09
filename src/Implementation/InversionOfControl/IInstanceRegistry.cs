// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public interface IInstanceRegistry
    {
        object? GetInstance(Type serviceType);

        void RegisterSingleton<TAbstraction>(Func<TAbstraction> factory)
            where TAbstraction : class;

        void RegisterTransient<TAbstraction>(Func<TAbstraction> factory)
            where TAbstraction : class;
    }
}