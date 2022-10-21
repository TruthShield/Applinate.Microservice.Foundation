// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class RequestInterceptorAttribute : Attribute
    {
        public RequestInterceptorAttribute(int ordinal)
        {
            Ordinal = ordinal;
        }

        public int Ordinal { get; }
    }
}