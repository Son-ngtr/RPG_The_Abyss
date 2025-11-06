using System;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;

    public float GetValue()
    {
        return baseValue;
    }

    // Buff or items affecting stats can be added here in the future
    // All calculations for buffs or items can be added here in the future
}
