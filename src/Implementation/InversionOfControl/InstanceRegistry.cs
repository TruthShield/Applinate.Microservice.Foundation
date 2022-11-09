// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{

    internal static class InstanceRegistry
    {
        internal static IInstanceRegistry Instance { get; set; } = new EmptyInstanceRegistry();

        //
        // Summary:
        //     Gets the service object of the specified type.
        //
        // Parameters:
        //   serviceType:
        //     An object that specifies the type of service object to get.
        //
        // Returns:
        //     A service object of type serviceType. -or- null if there is no service object
        //     of type serviceType.
        internal static object? GetInstance(Type serviceType) => 
            Instance.Get(serviceType); //UNDONE: check instance exists and

        internal static TAbstraction GetInstance<TAbstraction>() => 
            (TAbstraction)(Instance.Get(typeof(TAbstraction)) ?? default); // UNDONE: check for null and throw

        internal static void RegisterInstance<TAbstraction>(
            Func<TAbstraction> factory, 
            InstanceLifetime lifetime = InstanceLifetime.Transient)
            where TAbstraction : class =>
            Instance.Register(factory);
    }
}