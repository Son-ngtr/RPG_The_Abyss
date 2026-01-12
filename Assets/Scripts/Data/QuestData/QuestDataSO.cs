using UnityEditor;
using UnityEngine;

public enum RewardType 
{ 
    Merchant, 
    BlackSmith, 
    None 
}

public enum QuestTargetID
{
    Enemy_Skeleton,
    BlackSmithWokang,
    Merchant_Galim,

}

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/New Quest", fileName = "Quest - ")]
public class QuestDataSO : ScriptableObject
{
    public string questSaveID;

    [Space]
    public string questName;
    [TextArea] public string questDescription;
    [TextArea] public string questGoal;

    public QuestTargetID questTargetID; // Enemy name, item name, location name, etc.
    public int requiredAmount;

    [Header("Rewards")]
    public RewardType rewardType;
    public Inventory_Item[] rewardItems;


    private void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        questSaveID = AssetDatabase.AssetPathToGUID(path);
    }
}
