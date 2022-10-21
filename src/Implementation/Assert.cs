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

        public static void IsNotNullOrEmpty<T>(IEnumerable<T> parameter, string parameterName)
        {
            Assert.IsNotNull(parameter, parameterName);

            if(! parameter.Any())
            {
                throw new ArgumentOutOfRangeException(parameterName + " must have at least one item.");
            }    
        }
    }
}