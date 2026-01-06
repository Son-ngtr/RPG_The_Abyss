using System;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    public ItemDataSO ItemData;

    public Inventory_Item(ItemDataSO itemData)
    {
        this.ItemData = itemData;
    }
}
