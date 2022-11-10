// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public interface IInstanceRegistry
    {
        /// <summary>
        /// Gets an instance of the specified interfaceType.
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        object? Get(Type interfaceType);

        /// <summary>
        /// Registeres the strategy for instantiation
        /// of a class that imlements the specified interface.
        /// </summary>
        /// <typeparam name="TAbstraction">The interface of the class to be instantiated.</typeparam>
        /// <param name="factory"></param>
        /// <param name="lifetime"></param>
        void Register<TAbstraction>(
            Func<TAbstraction> factory, 
            InstanceLifetime lifetime = InstanceLifetime.Transient)
            where TAbstraction : class;
    }
}