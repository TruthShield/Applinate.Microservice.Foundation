// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class InitializationPriorityAttribute : Attribute
    {
        public InitializationPriorityAttribute(int ordinal)
        {
            Ordinal = ordinal;
        }

        public int Ordinal { get; }
    }
}