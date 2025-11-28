using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    private bool needToBeRecalculated = true;
    private float finalValue;

    public float GetValue()
    {
        if (needToBeRecalculated)
        {
            finalValue = GetFinalValue();
            needToBeRecalculated = false;
        }

        return finalValue;
    }

    public void AddModifier(float value, string source)
    {
        StatModifier modifier = new StatModifier(value, source);
        modifiers.Add(modifier);
        needToBeRecalculated = true;
    }
    // Buff or items affecting stats can be added here in the future
    // All calculations for buffs or items can be added here in the future

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source);
        needToBeRecalculated = true;
    }

    private float GetFinalValue()
    {
        float finalValue = baseValue;

        foreach (var modifier in modifiers)
        {
            finalValue += modifier.value;
        }

        return finalValue;
    }

    public void SetBaseValue(float value)
    {
        baseValue = value;
    }
}

[Serializable]
public class StatModifier
{
    public float value;
    public String source;

    public StatModifier(float value, String source)
    {
        this.value = value;
        this.source = source;
    }
}
