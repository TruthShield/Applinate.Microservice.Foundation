// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceType type)
        {
            ServiceType = type;
        }

        public ServiceType ServiceType { get; }
    }
}