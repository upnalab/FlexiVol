using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
// ReSharper disable CheckNamespace

[Serializable, DebuggerDisplay("Count = {Count}")]
public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    [FormerlySerializedAs("_Buckets")] [SerializeField, HideInInspector]
    private int[] buckets;
    [FormerlySerializedAs("_HashCodes")] [SerializeField, HideInInspector]
    private int[] hashCodes;
    [FormerlySerializedAs("_Next")] [SerializeField, HideInInspector]
    private int[] next;
    [FormerlySerializedAs("_Count")] [SerializeField, HideInInspector]
    private int count;
    [FormerlySerializedAs("_Version")] [SerializeField, HideInInspector]
    private int version;
    [FormerlySerializedAs("_FreeList")] [SerializeField, HideInInspector]
    private int freeList;
    [FormerlySerializedAs("_FreeCount")] [SerializeField, HideInInspector]
    private int freeCount;
    [FormerlySerializedAs("_Keys")] [SerializeField, HideInInspector]
    private TKey[] keys;
    [FormerlySerializedAs("_Values")] [SerializeField, HideInInspector]
    private TValue[] values;

    private readonly IEqualityComparer<TKey> _comparer;

    // Mainly for debugging purposes - to get the key-value pairs display
    public Dictionary<TKey, TValue> AsDictionary => new Dictionary<TKey, TValue>(this);

    public int Count => count - freeCount;

    public TValue this[TKey key, TValue defaultValue]
    {
        get
        {
            int index = FindIndex(key);
            return index >= 0 ? values[index] : defaultValue;
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            int index = FindIndex(key);
            if (index >= 0)
                return values[index];
            throw new KeyNotFoundException(key.ToString());
        }

        set => Insert(key, value, false);
    }

    public SerializableDictionary()
        : this(0, null)
    {
    }

    public SerializableDictionary(int capacity)
        : this(capacity, null)
    {
    }

    public SerializableDictionary(IEqualityComparer<TKey> comparer)
        : this(0, comparer)
    {
    }

    public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException("capacity");

        Initialize(capacity);

        _comparer = (comparer ?? EqualityComparer<TKey>.Default);
    }

    public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer = null)
        : this(dictionary?.Count ?? 0, comparer)
    {
        if (dictionary == null)
            throw new ArgumentNullException(nameof(dictionary));

        foreach (KeyValuePair<TKey, TValue> current in dictionary)
            Add(current.Key, current.Value);
    }

    public bool ContainsValue([CanBeNull] TValue value)
    {
        if (value == null)
        {
            for (int i = 0; i < count; i++)
            {
                if (hashCodes[i] >= 0 && values[i] == null)
                    return true;
            }
        }
        else
        {
            EqualityComparer<TValue> defaultComparer = EqualityComparer<TValue>.Default;
            for (var i = 0; i < count; i++)
            {
                if (hashCodes[i] >= 0 && defaultComparer.Equals(values[i], value))
                    return true;
            }
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return FindIndex(key) >= 0;
    }

    public void Clear()
    {
        if (count <= 0)
            return;

        for (var i = 0; i < buckets.Length; i++)
            buckets[i] = -1;

        Array.Clear(keys, 0, count);
        Array.Clear(values, 0, count);
        Array.Clear(hashCodes, 0, count);
        Array.Clear(next, 0, count);

        freeList = -1;
        count = 0;
        freeCount = 0;
        version++;
    }

    public void Add(TKey key, TValue value)
    {
        Insert(key, value, true);
    }

    private void Resize(int newSize, bool forceNewHashCodes)
    {
        var bucketsCopy = new int[newSize];
        for (var i = 0; i < bucketsCopy.Length; i++)
            bucketsCopy[i] = -1;

        var keysCopy = new TKey[newSize];
        var valuesCopy = new TValue[newSize];
        var hashCodesCopy = new int[newSize];
        var nextCopy = new int[newSize];

        Array.Copy(values, 0, valuesCopy, 0, count);
        Array.Copy(keys, 0, keysCopy, 0, count);
        Array.Copy(hashCodes, 0, hashCodesCopy, 0, count);
        Array.Copy(next, 0, nextCopy, 0, count);

        if (forceNewHashCodes)
        {
            for (var i = 0; i < count; i++)
            {
                if (hashCodesCopy[i] != -1)
                    hashCodesCopy[i] = (_comparer.GetHashCode(keysCopy[i]) & 2147483647);
            }
        }

        for (var i = 0; i < count; i++)
        {
            int index = hashCodesCopy[i] % newSize;
            nextCopy[i] = bucketsCopy[index];
            bucketsCopy[index] = i;
        }

        buckets = bucketsCopy;
        keys = keysCopy;
        values = valuesCopy;
        hashCodes = hashCodesCopy;
        next = nextCopy;
    }

    private void Resize()
    {
        Resize(PrimeHelper.ExpandPrime(count), false);
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int hash = _comparer.GetHashCode(key) & 2147483647;
        int index = hash % buckets.Length;
        int num = -1;
        for (int i = buckets[index]; i >= 0; i = next[i])
        {
            if (hashCodes[i] == hash && _comparer.Equals(keys[i], key))
            {
                if (num < 0)
                    buckets[index] = next[i];
                else
                    next[num] = next[i];

                hashCodes[i] = -1;
                next[i] = freeList;
                keys[i] = default(TKey);
                values[i] = default(TValue);
                freeList = i;
                freeCount++;
                version++;
                return true;
            }
            num = i;
        }
        return false;
    }

    private void Insert(TKey key, TValue value, bool add)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (buckets == null)
            Initialize(0);

        int hash = _comparer.GetHashCode(key) & 2147483647;
        int index = hash % buckets.Length;
        for (int i = buckets[index]; i >= 0; i = next[i])
        {
            if (hashCodes[i] == hash && _comparer.Equals(keys[i], key))
            {
                if (add)
                    throw new ArgumentException("Key already exists: " + key);

                values[i] = value;
                version++;
                return;
            }
        }
        int num2;
        if (freeCount > 0)
        {
            num2 = freeList;
            freeList = next[num2];
            freeCount--;
        }
        else
        {
            if (count == keys.Length)
            {
                Resize();
                index = hash % buckets.Length;
            }
            num2 = count;
            count++;
        }
        hashCodes[num2] = hash;
        next[num2] = buckets[index];
        keys[num2] = key;
        values[num2] = value;
        buckets[index] = num2;
        version++;

        //if (num3 > 100 && HashHelpers.IsWellKnownEqualityComparer(comparer))
        //{
        //    comparer = (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(comparer);
        //    Resize(entries.Length, true);
        //}
    }

    private void Initialize(int capacity)
    {
        int prime = PrimeHelper.GetPrime(capacity);

        buckets = new int[prime];
        for (var i = 0; i < buckets.Length; i++)
            buckets[i] = -1;

        keys = new TKey[prime];
        values = new TValue[prime];
        hashCodes = new int[prime];
        next = new int[prime];

        freeList = -1;
    }

    private int FindIndex(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (buckets == null) return -1;
        
        int hash = _comparer.GetHashCode(key) & 2147483647;
        for (int i = buckets[hash % buckets.Length]; i >= 0; i = next[i])
        {
            if (hashCodes[i] == hash && _comparer.Equals(keys[i], key))
                return i;
        }
        return -1;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = FindIndex(key);
        if (index >= 0)
        {
            value = values[index];
            return true;
        }
        value = default(TValue);
        return false;
    }

    private static class PrimeHelper
    {
        private static readonly int[] Primes = new int[]
        {
            3,
            7,
            11,
            17,
            23,
            29,
            37,
            47,
            59,
            71,
            89,
            107,
            131,
            163,
            197,
            239,
            293,
            353,
            431,
            521,
            631,
            761,
            919,
            1103,
            1327,
            1597,
            1931,
            2333,
            2801,
            3371,
            4049,
            4861,
            5839,
            7013,
            8419,
            10103,
            12143,
            14591,
            17519,
            21023,
            25229,
            30293,
            36353,
            43627,
            52361,
            62851,
            75431,
            90523,
            108631,
            130363,
            156437,
            187751,
            225307,
            270371,
            324449,
            389357,
            467237,
            560689,
            672827,
            807403,
            968897,
            1162687,
            1395263,
            1674319,
            2009191,
            2411033,
            2893249,
            3471899,
            4166287,
            4999559,
            5999471,
            7199369
        };

        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0) return candidate == 2;
            
            var num = (int)Math.Sqrt(candidate);
            for (var i = 3; i <= num; i += 2)
            {
                if (candidate % i == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static int GetPrime(int min)
        {
            if (min < 0)
                throw new ArgumentException("min < 0");

            foreach (int prime in Primes)
            {
                if (prime >= min)
                    return prime;
            }
            
            for (int i = min | 1; i < 2147483647; i += 2)
            {
                if (IsPrime(i) && (i - 1) % 101 != 0)
                    return i;
            }
            return min;
        }

        public static int ExpandPrime(int oldSize)
        {
            int num = 2 * oldSize;
            if (num > 2146435069 && 2146435069 > oldSize)
            {
                return 2146435069;
            }
            return GetPrime(num);
        }
    }

    public ICollection<TKey> Keys => keys.Take(Count).ToArray();

    public ICollection<TValue> Values => values.Take(Count).ToArray();

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int index = FindIndex(item.Key);
        return index >= 0 &&
            EqualityComparer<TValue>.Default.Equals(values[index], item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (index < 0 || index > array.Length)
            throw new ArgumentOutOfRangeException($"index = {index} array.Length = {array.Length}");

        if (array.Length - index < Count)
            throw new ArgumentException(
                $"The number of elements in the dictionary ({Count}) is greater than the available space from index to the end of the destination array {array.Length}.");

        for (int i = 0; i < count; i++)
        {
            if (hashCodes[i] >= 0)
                array[index++] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
        }
    }

    public bool IsReadOnly => false;

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return GetEnumerator();
    }

    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly SerializableDictionary<TKey, TValue> _dictionary;
        private readonly int _version;
        private int _index;

        public KeyValuePair<TKey, TValue> Current { get; private set; }

        internal Enumerator(SerializableDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _version = dictionary.version;
            Current = default(KeyValuePair<TKey, TValue>);
            _index = 0;
        }

        public bool MoveNext()
        {
            if (_version != _dictionary.version)
                throw new InvalidOperationException(
                    $"Enumerator version {_version} != Dictionary version {_dictionary.version}");

            while (_index < _dictionary.count)
            {
                if (_dictionary.hashCodes[_index] >= 0)
                {
                    Current = new KeyValuePair<TKey, TValue>(_dictionary.keys[_index], _dictionary.values[_index]);
                    _index++;
                    return true;
                }
                _index++;
            }

            _index = _dictionary.count + 1;
            Current = default(KeyValuePair<TKey, TValue>);
            return false;
        }

        void IEnumerator.Reset()
        {
            if (_version != _dictionary.version)
                throw new InvalidOperationException(
                    $"Enumerator version {_version} != Dictionary version {_dictionary.version}");

            _index = 0;
            Current = default(KeyValuePair<TKey, TValue>);
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}