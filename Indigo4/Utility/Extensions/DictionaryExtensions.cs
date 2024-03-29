﻿using Indigo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class DictionaryExtensions
{
    public static TValue ValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
        if (!dict.TryGetValue(key, out TValue result))
            return default(TValue);

        return result;
    }

    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFactory)
    {
        if (valueFactory == null)
            throw new ArgumentNullException(nameof(valueFactory));

        if (!dict.TryGetValue(key, out var value))
            dict[key] = value = valueFactory();

        return value;
    }

    public static TValue GetOrInsertNew<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        where TValue : new()
    {
        if (!dict.TryGetValue(key, out TValue result))
            return (dict[key] = FastFactory<TValue>.CreateInstance());

        return result;
    }
}