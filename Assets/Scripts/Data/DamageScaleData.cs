using System;
using UnityEngine;

[Serializable]
public class DamageScaleData
{
    [Header("DAMAGE")]
    public float physical = 1f;
    public float elemental = 1f;

    [Header("CHILL")]
    public float chillDuration = 3f;
    public float chillSlowMultiplier = 0.2f;

    [Header("BURN")]
    public float burnDuration = 3f;
    public float burnDamageScale = 1f;

    [Header("SHOCK")]
    public float shockDuration = 3f;
    public float shockDamageScale = 1f;
    public float shockCharge = 0.4f;
}
