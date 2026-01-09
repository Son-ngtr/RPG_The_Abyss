using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Equipment Item", fileName = "Equipment data - ")]
public class EquipmentDataSO : ItemDataSO
{
    [Header("ITEM MODIFIERS")]
    public ItemModifier[] modifiers;
}


[Serializable]
public class ItemModifier
{
    public StatType statType;
    public float value;
}