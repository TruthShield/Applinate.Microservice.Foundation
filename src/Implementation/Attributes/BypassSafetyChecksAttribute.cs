// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false)]
    public class BypassSafetyChecksAttribute : Attribute
    {
    }
}