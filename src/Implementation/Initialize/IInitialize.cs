// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Interface ILazyAppInitializer
    /// </summary>
    /// <remarks>
    /// any class implementing this interface will be called when the utility
    /// method <see cref="InitializationProvider"/>.<see cref="InitializationProvider.Initialize(IServiceCollection, bool)"/> is called.
    /// </remarks>
    public interface IInitialize // TODO: document in readme
    {
        bool SkipDuringTesting { get; }
        void Initialize(IServiceCollection services, bool testing = false);
    }
}