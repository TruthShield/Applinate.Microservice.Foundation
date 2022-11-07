// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceRequestAttribute : Attribute
    {
        public ServiceRequestAttribute(ServiceType type)
        {
            CommandType = type;
        }

        public ServiceType CommandType { get; }
    }

    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceType type)
        {
            CommandType = type;
        }

        public ServiceType CommandType { get; }
    }
}