// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public enum InstanceLifetime
    {
        Undefined = 0,
        /// <summary>
        /// The same instance is provided for every call.
        /// </summary>
        Singleton = 1,
        /// <summary>
        /// A new instance is provided for every call.
        /// </summary>
        Transient = 2
    }
}