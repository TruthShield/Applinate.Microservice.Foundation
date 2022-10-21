// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Collections.Immutable;

    public static class EnumerableExtensions
    {
        public static ImmutableArray<T> BuildImmutableArray<T>(this IEnumerable<T>? arg)
        {
            return (arg ?? Array.Empty<T>()).ToImmutableArray();
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> items, T appendedItem)
        {
            foreach (var item in items)
            {
                yield return item;
            }

            yield return appendedItem;
        }
    }
}