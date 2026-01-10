using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    public Inventory_Player playerInventory { get; private set; }
    public List<Inventory_Item> materialStash;

    public void CraftItem(Inventory_Item itemToCraft)
    {
        ComsumeMaterials(itemToCraft);
        playerInventory.AddItem(itemToCraft);
    }

    public bool CanCraftItem(Inventory_Item itemToCraft)
    {
        return HasEnoughMaterials(itemToCraft) && playerInventory.CanAddItem(itemToCraft);
    }

    // Comsume Meterials in all place that can stoge item (1. Inventory 2. Storage 3. Material Stash)
    private void ComsumeMaterials(Inventory_Item itemToCraft)
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
            var newItemToAdd = new Inventory_Item(itemToAdd.itemData);
            materialStash.Add(newItemToAdd);
        }

        TriggerUpdateUi();
        materialStash = materialStash.OrderBy(item => item.itemData.name).ToList();
    }

    public Inventory_Item stackableInStash(Inventory_Item itemToAdd)
    {
        return materialStash.Find(item => item.itemData == itemToAdd.itemData && item.CanAddStack());
        
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


    // SAVE AND LOAD SYSTEM
    public override void SaveData(ref GameData data)
    {
        base.SaveData(ref data);

        // Save storage items
        data.storageItems.Clear();
        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                if (data.storageItems.ContainsKey(saveID) == false)
                {
                    // New item, add it to the dictionary
                    data.storageItems[saveID] = 0;
                }

                data.storageItems[saveID] += item.stackSize;
            }
        }

        // Save material stash
        data.storageMaterials.Clear();
        foreach (var item in materialStash)
        {
            if (item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                if (data.storageMaterials.ContainsKey(saveID) == false)
                {
                    // New item, add it to the dictionary
                    data.storageMaterials[saveID] = 0;
                }

                data.storageMaterials[saveID] += item.stackSize;
            }
        }
    }

    public override void LoadData(GameData data)
    {
        // Clear current items
        itemList.Clear();
        materialStash.Clear();

        // Load storage items
        foreach (var item in data.storageItems)
        {
            string saveID = item.Key;
            int stackSize = item.Value;

            ItemDataSO itemData = itemDataBase.GetItemDataByID(saveID); // itemDataBase is ItemListDataSO reference to get ItemDataSO by saveID

            if (itemData == null)
            {
                Debug.LogWarning($"ItemData with saveID {saveID} not found in database.");
                continue;
            }

            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData); // Create new Inventory_Item instance everytime to avoid reference issues
                // AddItem will handle stacking logic
                AddItem(itemToLoad);
            }
        }

        // Load material stash
        foreach (var item in data.storageMaterials)
        {
            string saveID = item.Key;
            int stackSize = item.Value;

            ItemDataSO itemData = itemDataBase.GetItemDataByID(saveID); // itemDataBase is ItemListDataSO reference to get ItemDataSO by saveID

            if (itemData == null)
            {
                Debug.LogWarning($"ItemData with saveID {saveID} not found in database.");
                continue;
            }

            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData); // Create new Inventory_Item instance everytime to avoid reference issues
                // AddItem will handle stacking logic
                AddMaterialToStash(itemToLoad);
            }
        }

        TriggerUpdateUi();
    }
}
