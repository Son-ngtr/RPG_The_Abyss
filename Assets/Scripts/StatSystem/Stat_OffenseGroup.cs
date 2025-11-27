using System;
using UnityEngine;

[Serializable]
public class Stat_OffenseGroup
{
    // Physical Attack Power
    public Stat damage;
    public Stat critChance;
    public Stat critDamage;
    public Stat armorReduction;

    // Elemental Attack Power
    public Stat fireDamage;  
    public Stat iceDamage;
    public Stat lightningDamage;
}
