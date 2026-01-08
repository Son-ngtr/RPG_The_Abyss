using System.Collections.Generic;
using UnityEngine;

// INVENTORY OF PLAYER
public class Inventory_Player : Inventory_Base
{
    private Player player;
    public List<Inventory_EquipmentSlot> equipList;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<Player>();
    }

    public void TryEquipItem(Inventory_Item item)
    {
        Inventory_Item inventoryItem = FindItem(item.itemData);

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

        EquipItem(inventoryItem, slotToReplace);
        UnequipItem(itemToUnequip);
    }

    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        // Save current health percent to restore after stat changes
        float savedHealthPercent = player.health.GetHealthPercent();

        slot.equipedItem = itemToEquip;
        slot.equipedItem.AddModifiers(player.stats);
        slot.equipedItem.AddItemEffect(player);

        player.health.SetHealthToPercent(savedHealthPercent);

        RemoveItem(itemToEquip);
    }


    public void UnequipItem(Inventory_Item itemToUnequip)
    {
        // Check if there is space in inventory
        if (CanAddItem() == false)
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
