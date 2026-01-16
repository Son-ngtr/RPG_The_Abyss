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
    None,
    Enemy_Slime,
    Enemy_Archer,
    Enemy_Mage,
    Enemy_Boss_Reaper
}

public enum QuestType
{
    Kill,
    Talk,
    Deliver
}

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/New Quest", fileName = "Quest - ")]
public class QuestDataSO : ScriptableObject
{
    public string questSaveID;

    [Space]
    public QuestType questType;
    public string questName;
    [TextArea] public string questDescription;
    [TextArea] public string questGoal;

    public QuestTargetID questTargetID; // Enemy name, item name, location name, etc.
    public int requiredAmount;
    public ItemDataSO deliverItem; // Only for deliver quests

    [Header("Rewards")]
    public RewardType rewardType;
    public Inventory_Item[] rewardItems;


    private void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        questSaveID = AssetDatabase.AssetPathToGUID(path);
    }
}
