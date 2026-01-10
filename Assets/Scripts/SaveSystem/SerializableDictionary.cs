using System;
using System.Collections.Generic;
using UnityEngine;

// A serializable dictionary that can be used in Unity to store key-value pairs
// Cause unity's built-in serialization does not support dictionaries directly
[Serializable]
public class SerializableDictionary<Tkey, Tvalue> : Dictionary<Tkey, Tvalue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<Tkey> keys = new List<Tkey>();
    [SerializeField] private List<Tvalue> values = new List<Tvalue>();

    // Converts the two lists back into the dictionary after deserialization
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError("Deserialization Error: Keys and Values count mismatch.");
        }

        for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

    // Converts the dictionary into two lists for serialization
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<Tkey, Tvalue> pairs in this) // 'this' refers to the Dictionary
        {
            keys.Add(pairs.Key);
            values.Add(pairs.Value);
        }
    }
}
