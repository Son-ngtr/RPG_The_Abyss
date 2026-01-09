using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity_DropManager : MonoBehaviour
{
    [SerializeField] private GameObject itemDropPrefab;
    [SerializeField] private ItemListDataSO dropData;

    [Header("DROP RESTRICTIONS")]
    [SerializeField] private int maxRarityAmount = 1200;
    [SerializeField] private int maxItemDrops = 5;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            DropItems();
        }
    }

    public virtual void DropItems()
    {
        if (dropData == null)
        {
            Debug.Log("You need to assign drop data to " + gameObject.name);
            return;
        }

        List<ItemDataSO> droppedItems = RollDrops();
        int amountToDrop = Mathf.Min(droppedItems.Count, maxItemDrops);

        for (int i = 0; i < amountToDrop; i++)
        {
            CreateItemDrop(droppedItems[i]);
        }
    }

    protected void CreateItemDrop(ItemDataSO itemToDrop)
    {
        GameObject newItem = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        newItem.GetComponent<Object_ItemPickup>().SetupItem(itemToDrop);
    }

    // Rolls the drops based on drop chances and rarity limits
    public List<ItemDataSO> RollDrops()
    {
        List<ItemDataSO> possibleDrops = new List<ItemDataSO>();
        List<ItemDataSO> finalDrops = new List<ItemDataSO>();
        float maxRarityAmount = this.maxRarityAmount;

        // STEP 1: Get possible drops based on drop chance
        foreach (var item in dropData.itemDataList)
        {
            float dropChance = item.GetDropChance();

            if (Random.Range(0, 100) <= dropChance)
            {
                possibleDrops.Add(item);
            }
        }

        // STEP 2: SORT possible drops by rarity (highest to lowest)
            // For example, when u defeat a boss, you can get many possible items
            // So the game will give you the rarest items first because you deserve it (*)
        possibleDrops = possibleDrops.OrderByDescending(i => i.itemRarity).ToList();

        // STEP 3: Add items to final drop list until rarity limit of entiry reached
            // (*) But a boss can only drop items up to a certain rarity limit, :(( u cant force them to drop super rare items if they cant
            // Once you use all their rarity points, they cant drop any more items
        foreach (var item in possibleDrops)
        {
            if (maxRarityAmount > item.itemRarity)
            {
                finalDrops.Add(item);
                maxRarityAmount = maxRarityAmount - item.itemRarity;
            }
        }

        return finalDrops;
    }
}
