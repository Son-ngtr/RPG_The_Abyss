using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    public Inventory_Player playerInventory { get; private set; }
    public List<Inventory_Item> materialStash;


    // Comsume Meterials in all place that can stoge item (1. Inventory 2. Storage 3. Material Stash)
    public void ComsumeMaterials(Inventory_Item itemToCraft)
    {
        foreach (var requiredItem in itemToCraft.itemData.craftRecipe)
        {
            int amountToComsume = requiredItem.stackSize;

            amountToComsume -= ConsumedMatarialAmount(playerInventory.itemList, requiredItem);

            if (amountToComsume > 0)
            {
                amountToComsume -= ConsumedMatarialAmount(itemList, requiredItem);
            }

            if (amountToComsume > 0)
            {
                amountToComsume -= ConsumedMatarialAmount(materialStash, requiredItem);
            }
        }
    }

    // Consume Materials and Return amount consumed in specific Location (1. Inventory 2. Storage 3. Material Stash)
    private int ConsumedMatarialAmount(List<Inventory_Item> itemList, Inventory_Item neededItem)
    {
        int amountNeeded = neededItem.stackSize;
        int comsumedAmount = 0;

        foreach (var item in itemList)
        {
            if (item.itemData != neededItem.itemData)
            {
                continue;
            }

            // Check how many material will be removed
                // all the stack if need more than stack
                // just take a part of stack
            int removeAmount = Mathf.Min(item.stackSize, amountNeeded - comsumedAmount);
            item.stackSize -= removeAmount;
            comsumedAmount += removeAmount;

            if (item.stackSize <= 0)
            {
                itemList.Remove(item);
            }

            if (comsumedAmount >= amountNeeded)
            {
                break;
            }
        }

        return comsumedAmount;
    }


    public bool HasEnoughMaterials(Inventory_Item itemToCraft)
    {
        foreach (var requiredIMaterial in itemToCraft.itemData.craftRecipe)
        {
            if (GetAvaibleAmountOf(requiredIMaterial.itemData) < requiredIMaterial.stackSize)
            {
                return false;
            }
        }

        return true;
    }

    // Get total available amount of specific item from player inventory, storage inventory and material stash
    public int GetAvaibleAmountOf(ItemDataSO requiredItem)
    {
        int availableAmount = 0;

        foreach (var item in playerInventory.itemList)
        {
            if (item.itemData == requiredItem)
            {
                availableAmount += item.stackSize;
            }
        }

        foreach (var item in itemList)
        {
            if (item.itemData == requiredItem)
            {
                availableAmount += item.stackSize;
            }
        }

        foreach (var item in materialStash)
        {
            if (item.itemData == requiredItem)
            {
                availableAmount += item.stackSize;
            }
        }

        return availableAmount;
    }

    public void AddMaterialToStash(Inventory_Item itemToAdd)
    {
        var stackableItem = stackableInStash(itemToAdd);

        // Add to existing stack if possible
        if (stackableItem != null)
        {
            stackableItem.AddStack();
        }
        else
        {
            materialStash.Add(itemToAdd);
        }

        TriggerUpdateUi();
    }

    public Inventory_Item stackableInStash(Inventory_Item itemToAdd)
    {
        List<Inventory_Item> stackableItems = materialStash.FindAll(item => item.itemData == itemToAdd.itemData);
        foreach (var stackableItem in stackableItems)
        {
            if (stackableItem.CanAddStack())
            {
                return stackableItem;
            }
        }
        return null;
    }


    public void SetInventory(Inventory_Player inventory) => this.playerInventory = inventory;

    // Transfer item from player inventory to storage inventory
    public void FromPlayerToStorage(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;

        for (int i = 0; i < transferAmount; i++)
        {
            if (CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);

                playerInventory.RemoveOneItem(item);
                AddItem(itemToAdd);
            }
        }


        TriggerUpdateUi();
    }

    // Transfer item from storage inventory to player inventory
    public void FromStorageToPlayer(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;

        for (int i = 0; i < transferAmount; i++)
        {
            if (playerInventory.CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);

                RemoveOneItem(item);
                playerInventory.AddItem(itemToAdd);
            }
        }


        TriggerUpdateUi();
    }
}
