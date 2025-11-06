using System;
using UnityEngine;

[Serializable]
public class Stat_OffenseGroup
{
    // Physical Attack Power
    public Stat damage;
    public Stat critRate;
    public Stat critDamage;

    // Elemental Attack Power
    public Stat fireDamage;  
    public Stat iceDamage;
    public Stat lightningDamage;
}
