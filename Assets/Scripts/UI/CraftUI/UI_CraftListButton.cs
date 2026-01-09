using UnityEngine;

public class UI_CraftListButton : MonoBehaviour
{
    [SerializeField] private ItemListDataSO craftableItemList;

    private UI_CraftSlot[] craftSlots;


    public void SetCraftSlots(UI_CraftSlot[] craftSlots) => this.craftSlots = craftSlots;

    public void UpdateCraftSlots()
    {
        if (craftableItemList == null)
        {
            Debug.LogWarning("Craftable Item List is not assigned in UI_CraftListButton.");
            return;
        }

        foreach (var craftSlot in craftSlots)
        {
            craftSlot.gameObject.SetActive(false);
        }

        for (int i = 0; i < craftableItemList.itemDataList.Length; i++)
        {
            ItemDataSO itemData = craftableItemList.itemDataList[i];

            craftSlots[i].gameObject.SetActive(true);
            // Assign item data to the craft slot
            craftSlots[i].SetupButton(itemData);
        }
    }
}
