using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory;
    private UI_EquipSlot[] uiEquipSlots;

    [SerializeField] private UI_ItemSlotParent inventorySlotsarent;
    [SerializeField] private Transform uiEquipSlotParent;

    private void Awake()
    {
        // Use parent tranform containing Itemslot and EquipSlot separately cause equipslot inherit from itemslot
        uiEquipSlots = uiEquipSlotParent.GetComponentsInChildren<UI_EquipSlot>();

        inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update SLOTS
            // Inventory - List of item in player's baggage
            // Equipments - List of item that player wearing
        inventorySlotsarent.UpdateSlots(inventory.itemList);
        UpdateEquipmentSlots();
    }

    private void UpdateEquipmentSlots()
    {
        List<Inventory_EquipmentSlot> playerEquipList = inventory.equipList;

        for (int i = 0; i < uiEquipSlots.Length; i++)
        {
            var playerEquipSlot = playerEquipList[i];

            if (playerEquipSlot.HasItem() == false)
            {
                uiEquipSlots[i].UpdateSlot(null);
            }
            else
            {
                uiEquipSlots[i].UpdateSlot(playerEquipSlot.equipedItem);
            }
        }
    }

}
