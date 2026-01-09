using UnityEngine;


[CreateAssetMenu(menuName = "RPG Setup/Item Data/Material Item", fileName = "Material data - ")]

public class ItemDataSO : ScriptableObject
{
    [Header("MERCHANT DETAILS")]
    public int itemPrice = 100;
    public int minStackSizeAtShop = 1;
    public int maxStackSizeAtShop = 1;

    [Header("ITEM DETAILS")]
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public int maxStackSize = 1;


    [Header("Item Effects")]
    public ItemEffect_DataSO itemEffect;


    [Header("CRAFT DETAILS")]
    public Inventory_Item[] craftRecipe;
}
