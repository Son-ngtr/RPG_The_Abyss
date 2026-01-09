using System;
using System.Text;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    private String itemID;

    public ItemDataSO itemData;
    public int stackSize = 1;

    public ItemModifier[] modifiers {  get; private set; }
    public ItemEffect_DataSO itemEffect;

    public int buyPrice { get; private set; }
    public float sellPrice { get; private set; }
    public float depreciationRate = 0.65f; // 50% of buy price

    public Inventory_Item(ItemDataSO itemData)
    {
        this.itemData = itemData;

        modifiers = EquimentData()?.modifiers; // Check if item is equiment (contain modifiers)
        itemEffect = itemData.itemEffect; // Get item effect from item data

        buyPrice = itemData.itemPrice;
        sellPrice = buyPrice * depreciationRate;

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

    public void AddItemEffect(Player player)
    {
        itemEffect?.SubScribeToPlayerEvents(player);
    }

    public void RemoveItemEffect(Player player)
    {
        itemEffect?.UnSubScribeToPlayerEvents(player);
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


    public string GetItemInfo()
    {
        StringBuilder sb = new StringBuilder();

        if (itemData.itemType == ItemType.Material)
        {
            sb.AppendLine("");
            sb.AppendLine("Used for crafting");
            sb.AppendLine("");
            sb.AppendLine("");
            return sb.ToString();
        }


        if (itemData.itemType == ItemType.Comsumable)
        {

            sb.AppendLine("");
            sb.AppendLine(itemEffect.effectDescription);
            sb.AppendLine("");
            sb.AppendLine("");
            return sb.ToString();
        }


        sb.AppendLine("");

        foreach (var mod in modifiers)
        {
            string modType = GetStatNameByType(mod.statType);
            string modValue = IsPercentageStat(mod.statType) ? mod.value.ToString() + "%" : mod.value.ToString();
            sb.AppendLine("+ " + modValue + " " + modType);
        }

        if (itemEffect != null)
        {
            sb.AppendLine("");
            sb.AppendLine("Unique effect:");
            sb.AppendLine(itemEffect.effectDescription);
        }

        sb.AppendLine("");
        sb.AppendLine("");

        return sb.ToString();
    }


    private string GetStatNameByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return "Max Health";
            case StatType.HealthRegen: return "Health Regenaration";
            case StatType.Strength: return "Strength";
            case StatType.Agility: return "Agility";

            case StatType.Intelligence: return "Intelligence";
            case StatType.Vitality: return "Vitality";
            case StatType.AttackSpeed: return "Attack Speed";
            case StatType.Damage: return "Damage";
            case StatType.CritChance: return "Critical Chance";
            case StatType.CritPower: return "Critical Power";

            case StatType.ArmorReduction: return "Armor Reduction";
            case StatType.FireDamage: return "Fire Damage";
            case StatType.IceDamage: return "Ice Damage";
            case StatType.LightningDamage: return "Lightning Damage";

            case StatType.Armor: return "Armor";
            case StatType.Evasion: return "Evasion";
            case StatType.IceResistance: return "Ice Resistance";
            case StatType.FireResistance: return "Fire Resistance";
            case StatType.LightningResistance: return "Lightning Resistance";

            default: return "Unknown Stat";
        }
    }

    private bool IsPercentageStat(StatType type)
    {
        switch (type)
        {
            case StatType.CritChance:
            case StatType.CritPower:
            case StatType.AttackSpeed:
            case StatType.ArmorReduction:
            case StatType.Evasion:
            case StatType.IceResistance:
            case StatType.FireResistance:
            case StatType.LightningResistance:
                return true;
            default:
                return false;
        }
    }
}
