using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/Quest Database", fileName = "QUEST DATABASE")]

public class QuestDatabaseSO : ScriptableObject
{   
    public QuestDataSO[] allQuests;

    public QuestDataSO GetQuestByID(string questID)
    {
        return allQuests.FirstOrDefault(quest => quest != null && quest.questSaveID == questID);
    }

#if UNITY_EDITOR
    [ContextMenu("Auto - fill with all QuestDataSO")]
    public void CollectItemData()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemDataSO"); // Find all ItemDataSO assets

        allQuests = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<QuestDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(quest => quest != null)
            .ToArray();

        EditorUtility.SetDirty(this); // Mark the ScriptableObject as dirty to ensure changes are saved
        AssetDatabase.SaveAssets(); // Save the changes to the asset database
    }
#endif
}
