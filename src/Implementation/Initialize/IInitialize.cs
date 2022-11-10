// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    /// <summary>
    /// Interface IInitialize
    /// </summary>
    /// <remarks>
    /// any class implementing this interface will be called when the utility
    /// method <see cref="InitializationProvider"/>.<see cref="InitializationProvider.Initialize(IServiceCollection, bool)"/> is called.
    /// </remarks>
    public interface IInitialize
    {
        bool SkipDuringTesting { get; }
        void Initialize(bool testing = false);
    }
}