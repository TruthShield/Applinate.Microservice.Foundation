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
        internal static object? GetInstance(Type serviceType) => Instance.GetInstance(serviceType); //UNDONE: check instance exists and

        internal static T GetInstance<T>() => (T)(Instance.GetInstance(typeof(T)) ?? default); // UNDONE: check for null and throw

        internal static void RegisterSingleton<TAbstraction, TConcretion>()
            where TAbstraction : class
            where TConcretion : class, TAbstraction =>
            Instance.RegisterSingleton<TAbstraction, TConcretion>();

        [Obsolete]
        internal static void RegisterSingleton<TAbstraction>(Func<TAbstraction> factory)
            where TAbstraction : class =>
            Instance.RegisterSingleton<TAbstraction>(factory);

        internal static void RegisterSingleton<TAbstraction, TConcretion>(Func<object, TConcretion> factory)
            where TAbstraction : class
            where TConcretion : class, TAbstraction =>
            Instance.RegisterSingleton<TAbstraction, TConcretion>(factory);

        [Obsolete]
        internal static void RegisterTransient<TAbstraction>(Func<TAbstraction> factory)
            where TAbstraction : class =>
            Instance.RegisterTransient<TAbstraction>(factory);
    }

}