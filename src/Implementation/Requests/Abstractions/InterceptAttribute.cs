// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class InterceptAttribute : Attribute
    {
        public InterceptAttribute(int ordinal = int.MaxValue)
        {
            Ordinal = ordinal;
        }

        public int Ordinal { get; }
    }
}