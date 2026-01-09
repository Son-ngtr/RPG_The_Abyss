using System.Collections.Generic;
using UnityEngine;

// Inventory Base for Itemlist and ability to add/remove items
public class Inventory_Merchant : Inventory_Base
{
    private Inventory_Player playerInventory;

    [SerializeField] private ItemListDataSO shopData;
    [SerializeField] private int minItemsAmount = 4;


    protected override void Awake()
    {
        base.Awake();

        FillShopList();
    }

    public void TryBuyItem(Inventory_Item itemToBuy, bool buyFullStack)
    {
        int amountToBuy = buyFullStack ? itemToBuy.stackSize : 1;

        for (int i = 0; i < amountToBuy; i++)
        {
            if (playerInventory.gold < itemToBuy.buyPrice)
            {
                Debug.Log("Not enough money");
                return;
            }

            if (itemToBuy.itemData.itemType == ItemType.Material)
            {
                playerInventory.storage.AddMaterialToStash(itemToBuy);
            }
            else
            {
                if (playerInventory.CanAddItem(itemToBuy))
                {
                    var itemToAdd = new Inventory_Item(itemToBuy.itemData);
                    playerInventory.AddItem(itemToAdd);
                }
            }

            playerInventory.gold -= itemToBuy.buyPrice;
            RemoveOneItem(itemToBuy);
        }

        TriggerUpdateUi();
    }

    public void TrySellItem(Inventory_Item itemToSell, bool sellFullStack)
    {
        int amountToSell = sellFullStack ? itemToSell.stackSize : 1;

        for (int i = 0; i < amountToSell; i++)
        {
            int sellPrice = Mathf.FloorToInt(itemToSell.sellPrice);

            playerInventory.gold += itemToSell.buyPrice;
            playerInventory.RemoveOneItem(itemToSell);
        }

        TriggerUpdateUi();
    }

    public void FillShopList()
    {
        itemList.Clear();
        List<Inventory_Item> possibleItems = new List<Inventory_Item>();

        foreach (var itemData in shopData.itemDataList)
        {
            int randomizedStack = Random.Range(itemData.minStackSizeAtShop, itemData.maxStackSizeAtShop + 1);
            int finalStack = Mathf.Clamp(randomizedStack, 1, itemData.maxStackSize);

            Inventory_Item itemToAdd = new Inventory_Item(itemData);
            itemToAdd.stackSize = finalStack; 

            possibleItems.Add(itemToAdd);
        }

        int RandomItemAmount = Random.Range(minItemsAmount, maxInventorySize + 1);
        int finalAmount = Mathf.Clamp(RandomItemAmount, 1, possibleItems.Count);

        for (int i = 0; i < finalAmount; i++)
        {
            var randomIndex = Random.Range(0, possibleItems.Count);
            var item = possibleItems[randomIndex];

            if (CanAddItem(item))
            {
                possibleItems.Remove(item);
                AddItem(item);
            }
        }

        TriggerUpdateUi();
    }

    public void SetInventory(Inventory_Player inventory) => this.playerInventory = inventory;
}
