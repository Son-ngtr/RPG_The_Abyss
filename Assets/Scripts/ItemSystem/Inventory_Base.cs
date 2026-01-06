using System;
using System.Collections.Generic;
using UnityEngine;



public class Inventory_Base : MonoBehaviour
{
    public event Action OnInventoryChange;

    public int maxInventorySize = 10;
    public List<Inventory_Item> itemList = new List<Inventory_Item>();


    protected virtual void Awake()
    {

    }

    public bool CanAddItem() => itemList.Count < maxInventorySize;

    public void AddItem(Inventory_Item itemToAdd)
    {
        // Everytime we try to add item --> search item in inventory, and if it can stack or not
            // --> then in each case, we increase stack or add item
        Inventory_Item itemInInventory = FindItem(itemToAdd.itemData);
        if (itemInInventory != null && itemInInventory.CanAddStack())
        {
            itemInInventory.AddStack();
        }
        else
        {
            itemList.Add(itemToAdd);
        }

        OnInventoryChange?.Invoke();
    }

    public void RemoveItem(Inventory_Item itemToRemove)
    {
        itemList.Remove(FindItem(itemToRemove.itemData));
        OnInventoryChange?.Invoke();
    }

    public Inventory_Item FindItem(ItemDataSO itemData)
    {
        return itemList.Find(item => item.itemData == itemData);
    }
}
