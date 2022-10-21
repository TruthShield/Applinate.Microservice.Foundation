// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceType type)
        {
            CommandType = type;
        }

        public ServiceType CommandType { get; }
    }
}