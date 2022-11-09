// Copyright (c) TruthShield, LLC. All rights reserved.
using System.Diagnostics;
using System.Reflection;

namespace Applinate
{
    internal static class MethodInfoExtensions
    {
        [DebuggerStepThrough]
        internal static async Task<T?> InvokeAsync<T>(this MethodInfo methodInfo, object obj, params object[] parameters)
        {
            dynamic? awaitable = methodInfo.Invoke(obj, parameters);
            await awaitable;
            return (T?) awaitable?.GetAwaiter().GetResult();
        }
    }
}