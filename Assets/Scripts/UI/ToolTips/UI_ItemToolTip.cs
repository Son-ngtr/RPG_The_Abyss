using System.Text;
using TMPro;
using UnityEngine;

public class UI_ItemToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemInfo;

    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private Transform merchantInfo;
    [SerializeField] private Transform inventoryInfo;

    public void ShowToolTip(bool show, RectTransform targetRect, Inventory_Item itemToShow, bool buyPrice = false, bool showMerchantInfo = false)
    {
        base.ShowToolTip(show, targetRect);

        merchantInfo.gameObject.SetActive(showMerchantInfo);
        inventoryInfo.gameObject.SetActive(!showMerchantInfo);

        int price = buyPrice ? itemToShow.buyPrice : Mathf.FloorToInt(itemToShow.sellPrice);
        int totalPrice = price * itemToShow.stackSize;

        string fullStackSize = ($"Price: {totalPrice}g.({price}x{itemToShow.stackSize})");
        string singleItemSize = ($"Price: {price}g.");

        itemPrice.text = itemToShow.stackSize > 1 ? fullStackSize : singleItemSize;
        itemType.text = itemToShow.itemData.itemType.ToString();
        itemInfo.text = itemToShow.GetItemInfo();

        string color = GetColorByRarity(itemToShow.itemData.itemRarity);

        itemName.text = GetColoredText(color, itemToShow.itemData.itemName);
    }

    private string GetColorByRarity(int rarity)
    {
        if (rarity <= 100) return "white"; // common
        if (rarity <= 300) return "green"; // uncommon
        if (rarity <= 600) return "blue";  // rare
        if (rarity <= 800) return "purple";// epic
        return "orange"; // legendary
    }
}
