using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftSlot : MonoBehaviour
{
    private ItemDataSO itemToCraft;
    [SerializeField] private UI_CraftPreview craftPreview;

    [SerializeField] private Image craftItemIcon;
    [SerializeField] private TextMeshProUGUI craftItemName;
    

    public void SetupButton(ItemDataSO craftData)
    {
        this.itemToCraft = craftData;
        craftItemIcon.sprite = itemToCraft.itemIcon;
        craftItemName.text = itemToCraft.itemName;
    }

    public void UpdateCraftPreview()
    {
        craftPreview.UpdateCraftPreview(itemToCraft);
    }
}
