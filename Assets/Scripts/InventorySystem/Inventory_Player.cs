using System;
using System.Collections.Generic;
using UnityEngine;

// INVENTORY OF PLAYER
public class Inventory_Player : Inventory_Base
{
    public event Action<int> OnQuickSlotUsed;
    public int gold = 10000;

    public List<Inventory_EquipmentSlot> equipList;
    public Inventory_Storage storage { get; private set; }

    [Header("QUICK ITEM SLOTS")]
    public Inventory_Item[] quickItems = new Inventory_Item[2];


    protected override void Awake()
    {
        base.Awake();

        storage = FindFirstObjectByType<Inventory_Storage>();
    }


    public void SetQuickItemsInSlot(int slotIndex, Inventory_Item itemToSet)
    {
        if (slotIndex < 0 || slotIndex >= quickItems.Length)
        {
            Debug.LogWarning("Invalid quick item slot index!");
            return;
        }

        quickItems[slotIndex - 1] = itemToSet;
        TriggerUpdateUi();
    }

    public void TryUseQuickItemInSlot(int passedSlotNumber)
    {
        if (passedSlotNumber < 0 || passedSlotNumber >= quickItems.Length)
        {
            Debug.LogWarning("Invalid quick item slot index!");
            return;
        }

        int slotNumber = passedSlotNumber - 1;
        var itemToUse = quickItems[slotNumber];

        if (itemToUse == null)
            return;

        TryUseItem(itemToUse);
        if (FindItem(itemToUse) == null)
        {
            quickItems[slotNumber] = FindSameItem(itemToUse);
        }

        TriggerUpdateUi();
        OnQuickSlotUsed?.Invoke(slotNumber);
    }

    public void TryEquipItem(Inventory_Item item)
    {
        Inventory_Item inventoryItem = FindItem(item);

        // Every Equipment Items Hast slotType === itemType
            // For example: itemtype weapon will only be used in slot weapon
        var matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);

        //STEP 1: TRY TO FIND EMPTY SLOT AND EQUIP ITEM
        foreach (var slot in matchingSlots)
        {
            if (slot.HasItem() == false)
            {
                EquipItem(inventoryItem, slot);
                return;
            }
        }

        //STEP 2: NO EMPTY SLOTS? REPLACE FIRST ONE
        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equipedItem;

        UnequipItem(itemToUnequip, slotToReplace != null);
        EquipItem(inventoryItem, slotToReplace);
    }

    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        // Save current health percent to restore after stat changes
        float savedHealthPercent = player.health.GetHealthPercent();

        slot.equipedItem = itemToEquip;
        slot.equipedItem.AddModifiers(player.stats);
        slot.equipedItem.AddItemEffect(player);

        player.health.SetHealthToPercent(savedHealthPercent);

        RemoveOneItem(itemToEquip);
    }


    public void UnequipItem(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        // Check if there is space in inventory
            // Check if we are replacing item ON STEP 2 of TryEquipItem --> so the logic can continue
        if (CanAddItem(itemToUnequip) == false && replacingItem == false)
        {
            Debug.Log("No space!");
            return;
        }

        float savedHealthPercent = player.health.GetHealthPercent();

        // Find the slot that has this item and clear it
        var slotToUnequip = equipList.Find(s => s.equipedItem == itemToUnequip);
        if (slotToUnequip != null)
        {
            slotToUnequip.equipedItem = null;
        }

        itemToUnequip.RemoveModifiers(player.stats);
        itemToUnequip.RemoveItemEffect(player);

        player.health.SetHealthToPercent(savedHealthPercent);
        AddItem(itemToUnequip);
    }
}
