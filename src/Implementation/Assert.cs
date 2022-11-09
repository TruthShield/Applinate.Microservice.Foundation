// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Runtime.CompilerServices;

    public static class Assert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNull<T>(T value, string parameterName)
        {
            if(value is null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void NoDuplicateValues(IEnumerable<string> enumerable, string errorMessage, string parameterName)
        {
            if(enumerable.Distinct(StringComparer.Ordinal).Take(enumerable.Count() + 1).Count() != 
                enumerable.Take(enumerable.Distinct(StringComparer.Ordinal).Take(enumerable.Count() + 1).Count() + 1).Count())
            {
                throw new ArgumentException(errorMessage, parameterName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinimumCount<T>(IEnumerable<T> items, int count, string errorMessage, string parameterName)
        {
             if(!items.Skip(count - 1).Any())
            {
                throw new ArgumentException(errorMessage, parameterName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrWhitespace(string? parameter, string parameterName)
        {
            if(string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentException(parameter + " can not be null or whitespace", parameterName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(IEnumerable<T> parameter, string parameterName)
        {
            Assert.IsNotNull(parameter, parameterName);

            if(! parameter.Any())
            {
                throw new ArgumentOutOfRangeException(parameterName + " must have at least one item.");
            }    
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IsInterface(Type interfaceType, string parameterName)
        {
            Assert.IsNotNull(interfaceType, parameterName);

            if(!interfaceType.IsInterface)
            {
                throw new ArgumentException(parameterName + " must be an interface", parameterName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IsInterface<TAbstraction>() where TAbstraction : class
        {
            if(! typeof(TAbstraction).IsInterface)
            {
                throw new ArgumentException($"the generic type {typeof(TAbstraction)} must be an interface");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IsNotUndefined(InstanceLifetime lifetime, string parameterName)
        {
            if(lifetime == InstanceLifetime.Undefined)
            {
                throw new ArgumentException($"the lifetime must be specified. {nameof(InstanceLifetime)}.{nameof(InstanceLifetime.Undefined)} is not valid for this operation.", parameterName);
            }
        }
    }
}