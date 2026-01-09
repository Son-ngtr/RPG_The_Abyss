using UnityEngine;

public class UI_Craft : MonoBehaviour
{
    [SerializeField] private UI_ItemSlotParent inventoryParent;
    private Inventory_Player playerInventory;

    private UI_CraftPreview craftPreviewUI;
    private UI_CraftSlot[] craftSlots;
    private UI_CraftListButton[] craftListButton;


    public void SetupCraftUI(Inventory_Storage storage)
    {
        playerInventory = storage.playerInventory;
        playerInventory.OnInventoryChange += UpdateUI;
        UpdateUI();

        craftPreviewUI = GetComponentInChildren<UI_CraftPreview>(); 
        craftPreviewUI.SetupCraftPreview(storage);
        SetupCraftListButtons();
    }


    private void SetupCraftListButtons()
    {
        craftSlots = GetComponentsInChildren<UI_CraftSlot>();
        craftListButton = GetComponentsInChildren<UI_CraftListButton>();

        foreach (var craftSlot in craftSlots)
        {
            craftSlot.gameObject.SetActive(false);
        }

        foreach (var button in craftListButton)
        {
            button.SetCraftSlots(craftSlots);
        }
    }

    private void UpdateUI() => inventoryParent.UpdateSlots(playerInventory.itemList);
}
