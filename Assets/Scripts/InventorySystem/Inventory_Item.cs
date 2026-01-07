using System;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    private String itemID;

    public ItemDataSO itemData;
    public int stackSize = 1;

    public ItemModifier[] modifiers {  get; private set; }
    public ItemEffect_DataSO itemEffect;

    public Inventory_Item(ItemDataSO itemData)
    {
        this.itemData = itemData;

        modifiers = EquimentData()?.modifiers; // Check if item is equiment (contain modifiers)
        itemEffect = itemData.itemEffect; // Get item effect from item data

        itemID = itemData.itemName + " - " + Guid.NewGuid(); // Use to specific what item, so when add or remove 1 item, it wont affect the effect of item with the same name
    }

    public void AddModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.AddModifier(mod.value, itemID);
        }
    }

    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.RemoveModifier(itemID);
        }
    }

    private EquipmentDataSO EquimentData()
    {
        if (itemData is EquipmentDataSO equiment)
        {
            return equiment;
        }

        return null;
    }

    public bool CanAddStack() => stackSize < itemData.maxStackSize;

    public void AddStack() => stackSize++;

    public void RemoveStack() => stackSize--;
}
