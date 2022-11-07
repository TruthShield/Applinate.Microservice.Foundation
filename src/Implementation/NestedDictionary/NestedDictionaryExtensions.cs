// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Collections.Generic;

    public static class NestedDictionaryExtensions
	{
        public static NestedDictionary<TKey1, TKey2, TValue> ToNestedDictionary<TSource, TKey1, TKey2, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey1> key1Selector,
            Func<TSource, TKey2> key2Selector,
            Func<TSource, TValue> valueSelector)
                where TKey1 : notnull
                where TKey2 : notnull
                 => 
                ToNestedDictionary(
                    source,
                    key1Selector,
                    key2Selector,
                    valueSelector,
                    null,
                    null);

        public static NestedDictionary<TKey1, TKey2, TValue> ToNestedDictionary<TSource, TKey1, TKey2, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey1> key1Selector,
            Func<TSource, TKey2> key2Selector,
            Func<TSource, TValue> valueSelector,
            IEqualityComparer<TKey1>? comparer1,
            IEqualityComparer<TKey2>? comparer2)
                where TKey1 : notnull
                where TKey2 : notnull

        {
            Assert.IsNotNull(source, nameof(source));
            Assert.IsNotNull(key1Selector, nameof(key1Selector));
            Assert.IsNotNull(key2Selector, nameof(key2Selector));
            Assert.IsNotNull(valueSelector, nameof(valueSelector));

            NestedDictionary<TKey1, TKey2, TValue> dictionary = 
                new NestedDictionary<TKey1, TKey2, TValue>(comparer1, comparer2);

			foreach (TSource element in source)
				dictionary.Add(
                    key1Selector(element),
                    key2Selector(element),
                    valueSelector(element));

			return dictionary;
		}


        public static NestedDictionary<TKey1, TKey2, TKey3, TElement> ToNestedDictionary<TSource, TKey1, TKey2, TKey3, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey1> key1Selector,
            Func<TSource, TKey2> key2Selector,
            Func<TSource, TKey3> key3Selector,
            Func<TSource, TElement> elementSelector)
                where TKey1 : notnull
                where TKey2 : notnull
                where TKey3 : notnull => 
            ToNestedDictionary(
                source,
                key1Selector,
                key2Selector,
                key3Selector,
                elementSelector,
                null,
                null,
                null);

        public static NestedDictionary<TKey1, TKey2, TKey3, TElement> ToNestedDictionary<TSource, TKey1, TKey2, TKey3, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey1> key1Selector,
            Func<TSource, TKey2> key2Selector,
            Func<TSource, TKey3> key3Selector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey1>? comparer1,
            IEqualityComparer<TKey2>? comparer2,
            IEqualityComparer<TKey3>? comparer3)
                where TKey1 : notnull
                where TKey2 : notnull
                where TKey3 : notnull
        {
            Assert.IsNotNull(source, nameof(source));
            Assert.IsNotNull(key1Selector, nameof(key1Selector));
            Assert.IsNotNull(key2Selector, nameof(key2Selector));
            Assert.IsNotNull(key3Selector, nameof(key3Selector));
            Assert.IsNotNull(elementSelector, nameof(elementSelector));

			var dictionary = new NestedDictionary<TKey1, TKey2, TKey3, TElement>(
                comparer1,
                comparer2,
                comparer3);

			foreach (TSource element in source)
				dictionary.Add(
                    key1Selector(element),
                    key2Selector(element),
                    key3Selector(element),
                    elementSelector(element));

			return dictionary;
		}
	}

}