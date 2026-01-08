using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Buff Effect", fileName = "Item effect data - Buff")]

public class ItemEffect_Buff : ItemEffect_DataSO
{
    [SerializeField] private BuffEffectData[] buffsToApply;
    [SerializeField] private float duration;
    [SerializeField] private string source = Guid.NewGuid().ToString();

    Player_Stats playerStats;


    public override bool CanBeUsed()
    {
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<Player_Stats>();      
        }

        if (playerStats.CanApplyBuff(source))
        {
            return true;
        }
        else
        {
            Debug.Log("Cannot apply same buff: " + source + " already active.");
            return false;
        }
    }

    public override void ExecuteEffect()
    {
        playerStats.ApplyBuff(buffsToApply, duration, source);
    }
}
