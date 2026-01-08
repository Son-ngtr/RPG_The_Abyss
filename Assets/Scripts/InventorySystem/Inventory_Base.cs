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


    public void TryUseItem(Inventory_Item itemToUse)
    {
        Inventory_Item comsumable = itemList.Find(item => item == itemToUse);

        if (comsumable == null)
        {
            return;
        }

        comsumable.itemEffect.ExecuteEffect();

        if (comsumable.stackSize > 1)
        {
            comsumable.RemoveStack();
        }
        else
        {
            RemoveItem(comsumable);
        }

        OnInventoryChange?.Invoke();
    }

    public bool CanAddItem() => itemList.Count < maxInventorySize;

    public Inventory_Item FindStackable(Inventory_Item itemToAdd)
    {
        List<Inventory_Item> stackableItems = itemList.FindAll(item => item.itemData == itemToAdd.itemData);

        foreach (var stackableItem in stackableItems)
        {
            if (stackableItem.CanAddStack())
            {
                return stackableItem;
            }
        }

        return null;
    }

    public void AddItem(Inventory_Item itemToAdd)
    {
        // Everytime we try to add item --> search item in inventory, and if it can stack or not
            // --> then in each case, we increase stack or add item
        //Inventory_Item itemInInventory = FindItem(itemToAdd.itemData);

        var existingStackable = FindStackable(itemToAdd);

        if (existingStackable != null)
        {
            existingStackable.AddStack();
        }
        else
        {
            itemList.Add(itemToAdd);
        }

        OnInventoryChange?.Invoke();
    }

    public void RemoveItem(Inventory_Item itemToRemove)
    {
        itemList.Remove(itemToRemove);
        OnInventoryChange?.Invoke();
    }

    public Inventory_Item FindItem(ItemDataSO itemData)
    {
        return itemList.Find(item => item.itemData == itemData);
    }


    public void TriggerUpdateUi()
    {
        OnInventoryChange?.Invoke();
    }
}
