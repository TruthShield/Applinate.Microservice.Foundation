// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Runtime.CompilerServices;

    internal static class InstanceRegistry
    {
        /// <summary>
        /// Injection point for behavior set during initialization
        /// if an implementation of <see cref="IInstanceRegistry"/>
        /// is found.
        /// </summary>
        internal static IInstanceRegistry Instance { get; set; } = new EmptyInstanceRegistry();

        internal static object GetInstance(Type interfaceType)
        {
            Assert.IsInterface(interfaceType, nameof(interfaceType));

            var response = Instance.Get(interfaceType); 

            if(response is null)
            {
                throw new InvalidOperationException($"there is no registered imlementation for {interfaceType}");
            }

            if(!response.GetType().IsAssignableTo(interfaceType))
            {
                throw new InvalidOperationException($"invalid registration for {nameof(interfaceType)}.  {response.GetType()} does not implement {nameof(interfaceType)}.");
            }

            return response;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TAbstraction GetInstance<TAbstraction>() => 
            (TAbstraction)GetInstance(typeof(TAbstraction));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void RegisterInstance<TAbstraction>(
            Func<TAbstraction> factory,
            InstanceLifetime lifetime = InstanceLifetime.Transient)
            where TAbstraction : class
        {
            Assert.IsInterface<TAbstraction>();
            Assert.IsNotUndefined(lifetime, nameof(lifetime));

            Instance.Register(factory, lifetime);
        }
    }
}