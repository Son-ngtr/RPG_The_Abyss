using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : Entity_Stats
{
    // Define list itemBuff cause can defy in scriptable object (item)
    private List<string> activeBuff = new List<string>();
    private Inventory_Player playerInventory;


    protected override void Awake()
    {
        base.Awake();

        playerInventory = GetComponent<Inventory_Player>();
    }


    public bool CanApplyBuff(string buffName)
    {
        return activeBuff.Contains(buffName) == false;
    }

    public void ApplyBuff(BuffEffectData[] buffsToApply, float duration, string source)
    {
        StartCoroutine(BuffCo(buffsToApply, duration, source));
    }

    private IEnumerator BuffCo(BuffEffectData[] buffsToApply, float duration, string source)
    {
        activeBuff.Add(source);

        foreach (var buff in buffsToApply)
        {
            GetStatByType(buff.type).AddModifier(buff.value, source);
        }

        yield return new WaitForSeconds(duration);

        foreach (var buff in buffsToApply)
        {
            GetStatByType(buff.type).RemoveModifier(source);
        }

        playerInventory.TriggerUpdateUi();
        activeBuff.Remove(source);
    }
}
