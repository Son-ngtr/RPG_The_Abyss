using UnityEngine;


[CreateAssetMenu(menuName = "RPG Setup/Item Data/Material Item", fileName = "Material data - ")]

public class ItemDataSO : ScriptableObject
{
    [Header("MERCHANT DETAILS")]
    public int itemPrice = 100;
    public int minStackSizeAtShop = 1;
    public int maxStackSizeAtShop = 1;

    [Header("DROP DETAILS")]
    [Range(0f, 1000f)]
    public int itemRarity = 100;
    [Range(0f, 100f)]
    public float dropChance;
    [Range(0f, 100f)]
    public float maxDropChance = 65f;

    [Header("ITEM DETAILS")]
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public int maxStackSize = 1;


    [Header("Item Effects")]
    public ItemEffect_DataSO itemEffect;


    [Header("CRAFT DETAILS")]
    public Inventory_Item[] craftRecipe;


    private void OnValidate()
    {
        dropChance = GetDropChance();
    }


    public float GetDropChance()
    {
        float maxRarity = 1000f;
        float chance = (maxRarity - itemRarity + 1) / maxRarity * 100f;
        
        return Mathf.Min(chance, maxDropChance);
    }
}
