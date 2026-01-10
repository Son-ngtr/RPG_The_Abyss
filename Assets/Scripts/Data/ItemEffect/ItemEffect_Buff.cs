using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Buff Effect", fileName = "Item effect data - Buff")]

public class ItemEffect_Buff : ItemEffect_DataSO
{
    [SerializeField] private BuffEffectData[] buffsToApply;
    [SerializeField] private float duration;
    [SerializeField] private string source = Guid.NewGuid().ToString();


    public override bool CanBeUsed(Player player)
    {
        if (player.stats.CanApplyBuff(source))
        {
            this.player = player;
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
        player.stats.ApplyBuff(buffsToApply, duration, source);
        player = null;
    }
}
