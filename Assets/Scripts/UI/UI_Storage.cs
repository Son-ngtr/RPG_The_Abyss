using UnityEngine;

public class UI_Storage : MonoBehaviour
{
    private Inventory_Storage storage;
    private Inventory_Player inventory;

    [SerializeField] private UI_ItemSlotParent inventoryParent;
    [SerializeField] private UI_ItemSlotParent storageParent;
    [SerializeField] private UI_ItemSlotParent materialStashParent;


    private void OnEnable()
    {
        UpdateUI();
    }


    public void SetupStorage(Inventory_Storage storage)
    {
        this.storage = storage;
        inventory = storage.playerInventory;

        storage.OnInventoryChange += UpdateUI;
        UpdateUI();

        UI_StorageSlot[] storageSlots = GetComponentsInChildren<UI_StorageSlot>();

        // Assign the storage reference to each storage slot
        foreach (var slot in storageSlots)
        {
            slot.SetStorage(storage);
        }
    }

    private void UpdateUI()
    {
        if (storage == null)
        {
            return;
        }

        // Update the UI elements to reflect the current state of the storage inventory
        // This could involve refreshing item slots, updating counts, etc.
        inventoryParent.UpdateSlots(inventory.itemList);
        storageParent.UpdateSlots(storage.itemList);
        materialStashParent.UpdateSlots(storage.materialStash);
    }
}
