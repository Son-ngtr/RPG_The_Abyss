using System.Collections.Generic;
using UnityEngine;

// INVENTORY OF PLAYER
public class Inventory_Player : Inventory_Base
{
    private Player player;
    public List<Inventory_EquipmentSlot> equipList;
    public Inventory_Storage storage { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<Player>();
        storage = FindFirstObjectByType<Inventory_Storage>();
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
