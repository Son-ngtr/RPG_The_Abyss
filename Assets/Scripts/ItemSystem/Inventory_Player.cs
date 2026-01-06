using System.Collections.Generic;
using UnityEngine;

// INVENTORY OF PLAYER
public class Inventory_Player : Inventory_Base
{
    private Entity_Stats playerStats;
    public List<Inventory_EquipmentSlot> equipList;

    protected override void Awake()
    {
        base.Awake();

        playerStats = GetComponent<Entity_Stats>();
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
        slot.equipedItem = itemToEquip;
        slot.equipedItem.AddModifiers(playerStats);

        RemoveItem(itemToEquip);
    }


    public void UnequipItem(Inventory_Item itemToUnequip)
    {
        if (CanAddItem() == false)
        {
            Debug.Log("No space!");
            return;
        }

        foreach (var slot in equipList)
        {
            if (slot.equipedItem == itemToUnequip)
            {
                slot.equipedItem = null;
                break;
            }
        }
        
        itemToUnequip.RemoveModifiers(playerStats);
        AddItem(itemToUnequip);
    }
}
