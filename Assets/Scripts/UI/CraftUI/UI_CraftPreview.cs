using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreview : MonoBehaviour
{
    private Inventory_Item itemToCraft;
    private Inventory_Storage storage;
    private UI_CraftPreviewSlot[] craftPreviewSlots;

    [Header("ITEM PREVIEW SETUP")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI buttonText;

    public void SetupCraftPreview(Inventory_Storage storage)
    {
        this.storage = storage;
        craftPreviewSlots = GetComponentsInChildren<UI_CraftPreviewSlot>();

        foreach (var slot in craftPreviewSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }


    public void ConfirmCraft()
    {
        if (itemToCraft == null)
        {
            buttonText.text = "Pick an item";
            return;
        }

        if (storage.CanCraftItem(itemToCraft))
        {
            storage.CraftItem(itemToCraft);
        }

        UpdateCraftPreviewSLot();
    }


    public void UpdateCraftPreview(ItemDataSO itemData)
    {
        itemToCraft = new Inventory_Item(itemData);

        itemIcon.sprite = itemData.itemIcon;
        itemName.text = itemData.itemName;
        itemDescription.text = itemToCraft.GetItemInfo();
        UpdateCraftPreviewSLot();
    }

    private void UpdateCraftPreviewSLot()
    {
        foreach (var slot in craftPreviewSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < itemToCraft.itemData.craftRecipe.Length; i++)
        {
            // Craft Recipe - List Of Items Required to Craft
            // StackSize in here is the amount required to craft
            Inventory_Item requiredItem = itemToCraft.itemData.craftRecipe[i];
            int availableAmount = storage.GetAvaibleAmountOf(requiredItem.itemData);
            int requiredAmount = requiredItem.stackSize;

            craftPreviewSlots[i].gameObject.SetActive(true);
            craftPreviewSlots[i].SetupPreviewSlot(requiredItem.itemData, availableAmount, requiredAmount);
        }
    }
}
