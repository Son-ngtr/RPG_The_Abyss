using System;
using System.Collections.Generic;
using UnityEngine;



public class Inventory_Base : MonoBehaviour, ISaveable
{
    public event Action OnInventoryChange;

    protected Player player;
    public int maxInventorySize = 10;
    public List<Inventory_Item> itemList = new List<Inventory_Item>();


    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }


    public void TryUseItem(Inventory_Item itemToUse)
    {
        Inventory_Item comsumable = itemList.Find(item => item == itemToUse);

        if (comsumable == null)
        {
            return;
        }

        if (comsumable.itemEffect.CanBeUsed(player) == false)
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
            RemoveOneItem(comsumable);
        }

        OnInventoryChange?.Invoke();
    }

    public bool CanAddItem(Inventory_Item itemToAdd)
    {
        bool hasStackablee = FindStackable(itemToAdd) != null;

        return hasStackablee || itemList.Count < maxInventorySize;
    }

    public Inventory_Item FindStackable(Inventory_Item itemToAdd)
    {
        return itemList.Find(item => item.itemData == itemToAdd.itemData && item.CanAddStack());
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

    public void RemoveOneItem(Inventory_Item itemToRemove)
    {
        Inventory_Item itemInInventory = itemList.Find(item => item == itemToRemove);

        if (itemInInventory.stackSize > 1)
        {
            itemInInventory.RemoveStack();
        }
        else
        {
            itemList.Remove(itemToRemove);
        }

        OnInventoryChange?.Invoke();
    }

    public void RemoveFullStack(Inventory_Item itemToRemove)
    {
        for (int i = 0; i < itemToRemove.stackSize; i++)
        {
            RemoveOneItem(itemToRemove);
        }
    }

    public Inventory_Item FindItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item == itemToFind);
    }

    public Inventory_Item FindSameItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item.itemData == itemToFind.itemData);
    }

    public void TriggerUpdateUi()
    {
        OnInventoryChange?.Invoke();
    }

    // SAVE AND LOAD GAME DATA
    public virtual void LoadData(GameData data)
    {
        
    }

    public virtual void SaveData(ref GameData data)
    {
        
    }
}
