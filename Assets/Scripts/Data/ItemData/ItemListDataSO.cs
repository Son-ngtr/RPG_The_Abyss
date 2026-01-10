using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item List", fileName = "List of Items - ")]

public class ItemListDataSO : ScriptableObject
{
    public ItemDataSO[] itemDataList;

    public ItemDataSO GetItemDataByID(string saveId)
    {
        return itemDataList.FirstOrDefault(item => item != null && item.saveID == saveId);
    }

#if UNITY_EDITOR
    [ContextMenu("Auto - fill with all ItemSataSo")]
    public void CollectItemData()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemDataSO"); // Find all ItemDataSO assets

        itemDataList = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ItemDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(item => item != null)
            .ToArray();

        EditorUtility.SetDirty(this); // Mark the ScriptableObject as dirty to ensure changes are saved
        AssetDatabase.SaveAssets(); // Save the changes to the asset database
    }
#endif
}
