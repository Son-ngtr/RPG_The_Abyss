using System;
using UnityEngine;

// PLAYER EQUIPMENT SLOT
    // Eachtime player wear sthg, the stat can be modified
[Serializable]
public class Inventory_EquipmentSlot
{
    public ItemType slotType;
    public Inventory_Item equipedItem;

    public Inventory_Item GetEquipedItem() => equipedItem;

    public bool HasItem() => equipedItem != null && equipedItem.itemData != null; // Check if equipedItem not null and its data not null
}
