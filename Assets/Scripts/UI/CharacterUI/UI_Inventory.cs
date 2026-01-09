using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory;

    [SerializeField] private UI_ItemSlotParent inventorySlotsarent;
    [SerializeField] private UI_EquipSlotParent equipSlotParent;

    private void Awake()
    {
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
        equipSlotParent.UpdateEquipmentSlots(inventory.equipList);
    }

}
