// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Collections.Generic;

    public class NestedDictionary<TKey1, TValue> : Dictionary<TKey1, TValue> where TKey1 : notnull
    {
        public NestedDictionary() : base()
        {
        }

        public NestedDictionary(int capacity) : base(capacity)
        {
        }

        public NestedDictionary(IEqualityComparer<TKey1> comparer) : base(comparer)
        {
        }

        public NestedDictionary(IDictionary<TKey1, TValue> dictionary) : base(dictionary)
        {
        }

        public NestedDictionary(int capacity, IEqualityComparer<TKey1> comparer) : base(capacity, comparer)
        {
        }

        public NestedDictionary(IDictionary<TKey1, TValue> dictionary, IEqualityComparer<TKey1> comparer) : base(dictionary, comparer)
        {
        }
    }

    public class NestedDictionary<TKey1, TKey2, TValue> : NestedDictionary<TKey1, NestedDictionary<TKey2, TValue>>
        where TKey1 : notnull
        where TKey2 : notnull
    {
        private int Capacity2 { get; }

        /// <summary>
        /// Initializes a new instance of this class that is empty, has the default initial capacity, and uses the default equality comparer for the key types.
        /// </summary>
        public NestedDictionary() : base() { }

        public NestedDictionary(int capacity1, int capacity2) : base(capacity1) => Capacity2 = capacity2;

        public NestedDictionary(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2) :
            base(comparer1) => Comparer2 = comparer2;

        public NestedDictionary(IDictionary<TKey1, NestedDictionary<TKey2, TValue>> dictionary) :
            base(dictionary)
        { }

        public NestedDictionary(int capacity1, int capacity2, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2) : base(capacity1, comparer1)
        {
            Capacity2 = capacity2;
            Comparer2 = comparer2;
        }

        public NestedDictionary(IDictionary<TKey1, NestedDictionary<TKey2, TValue>> dictionary, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2) :
            base(dictionary, comparer1) => Comparer2 = comparer2;

        public IEqualityComparer<TKey2>? Comparer2 { get; }

        public new NestedDictionary<TKey2, TValue> this[TKey1 key1]
        {
            get => TryGetValue(key1, out NestedDictionary<TKey2, TValue> dict) ? dict : base[key1] = new NestedDictionary<TKey2, TValue>(Capacity2, Comparer2);
            set => base[key1] = value;
        }

        public void Add(TKey1 key1, TKey2 key2, TValue value) =>
            this[key1].Add(key2, value);

        public bool ContainsKey(TKey1 key1, TKey2 key2) =>
            TryGetValue(key1, out var dict) && dict.ContainsKey(key2);

        public bool ContainsValue(TValue value) =>
            Values.OfType<NestedDictionary<TKey2, TValue>>()
            .Any(x => x.ContainsValue(value));

        public bool Remove(TKey1 key1, TKey2 key2) =>
            TryGetValue(key1, out var dict) && dict.Remove(key2);

        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
        {
            value = default;
            return TryGetValue(key1, out var dict) && dict.TryGetValue(key2, out value);
        }
    }

    public class NestedDictionary<TKey1, TKey2, TKey3, TValue> : 
        NestedDictionary<TKey1, NestedDictionary<TKey2, TKey3, TValue>>
        where TKey1 : notnull
        where TKey2 : notnull
        where TKey3 : notnull
    {
        private int Capacity2 { get; }
        private int Capacity3 { get; }

        public NestedDictionary() : base()
        {
        }

        public NestedDictionary(
            int capacity1, 
            int capacity2, 
            int capacity3) :
            base(capacity1)
        {
            Capacity2 = capacity2;
            Capacity3 = capacity3;
        }

        public NestedDictionary(
            IEqualityComparer<TKey1> comparer1, 
            IEqualityComparer<TKey2> comparer2, 
            IEqualityComparer<TKey3> comparer3) : 
            base(comparer1)
        {
            Comparer2 = comparer2;
            Comparer3 = comparer3;
        }

        public NestedDictionary(IDictionary<TKey1, NestedDictionary<TKey2, TKey3, TValue>> dictionary) : 
            base(dictionary)
        {
        }

        public NestedDictionary(
            int capacity1,
            int capacity2,
            int capacity3,
            IEqualityComparer<TKey1> comparer1,
            IEqualityComparer<TKey2> comparer2,
            IEqualityComparer<TKey3> comparer3) : base(capacity1, comparer1)
        {
            Capacity2 = capacity2;
            Capacity3 = capacity3;

            Comparer2 = comparer2;
            Comparer3 = comparer3;
        }

        public NestedDictionary(IDictionary<TKey1, NestedDictionary<TKey2, TKey3, TValue>> dictionary, IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2, IEqualityComparer<TKey3> comparer3) : base(dictionary, comparer1)
        {
            Comparer2 = comparer2;
            Comparer3 = comparer3;
        }

        public IEqualityComparer<TKey2>? Comparer2 { get; }

        public IEqualityComparer<TKey3>? Comparer3 { get; }

        public new NestedDictionary<TKey2, TKey3, TValue> this[TKey1 key1]
        {
            get => TryGetValue(key1, out var dict) ? dict : base[key1] = 
                new NestedDictionary<TKey2, TKey3, TValue>(
                    Capacity2, 
                    Capacity3, 
                    Comparer2, 
                    Comparer3);

            set => base[key1] = value;
        }

        public void Add(TKey1 key1, TKey2 key2, TKey3 key3, TValue value) =>
            this[key1].Add(key2, key3, value);

        public void Add(TKey1 key1, TKey2 key2, NestedDictionary<TKey3, TValue> dict) =>
            this[key1].Add(key2, dict);

        public bool ContainsKey(TKey1 key1, TKey2 key2, TKey3 key3) =>
            TryGetValue(key1, out var dict) && 
            dict.ContainsKey(key2, key3);

        public bool ContainsKey(TKey1 key1, TKey2 key2) =>
            TryGetValue(key1, out var dict) && 
            dict.ContainsKey(key2);

        public bool ContainsValue(TValue value) =>
            Values.OfType<NestedDictionary<TKey2, TKey3, TValue>>()
            .Any(x => x.ContainsValue(value));

        public bool Remove(TKey1 key1, TKey2 key2, TKey3 key3) =>
            TryGetValue(key1, out var dict)
                && dict.Remove(key2, key3);

        public bool Remove(TKey1 key1, TKey2 key2) =>
            TryGetValue(key1, out var dict) && 
                dict.Remove(key2);

        public bool TryGetValue(TKey1 key1, TKey2 key2, TKey3 key3, out TValue value)
        {
            value = default;
            return TryGetValue(key1, out var dict) && 
                dict.TryGetValue(key2, key3, out value);
        }

        public bool TryGetValue(TKey1 key1, TKey2 key2, out NestedDictionary<TKey3, TValue> value)
        {
            value = default;
            return TryGetValue(key1, out var dict) && 
                dict.TryGetValue(key2, out value);
        }
    }

}