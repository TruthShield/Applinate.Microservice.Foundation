// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceRequestAttribute : Attribute
    {
        public ServiceRequestAttribute(ServiceType type)
        {
            ServiceType = type;
        }

        public ServiceType ServiceType { get; }
    }
}