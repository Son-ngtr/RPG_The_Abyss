using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Player_QuestManager : MonoBehaviour, ISaveable
{
    public List<QuestData> activeQuests;
    public List<QuestData> completedQuests;

    private Entity_DropManager dropManager;

    [Header("QUEST DATABASE")]
    [SerializeField] private QuestDatabaseSO questDatabase;


    private void Awake()
    {
        dropManager = GetComponent<Entity_DropManager>();
    }

    public void TryGetRewardFrom(RewardType npcType)
    {
        List<QuestData> getRewardQuests = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            if (quest.CanGetReward() && quest.questDataSO.rewardType == npcType)
            {
                getRewardQuests.Add(quest);
            }
        }

        foreach (var quest in getRewardQuests)
        {
            GetQuestReward(quest.questDataSO);
            CompleteQuest(quest);
        }
    }

    private void GetQuestReward(QuestDataSO questDataSO)
    {
        foreach (var item in questDataSO.rewardItems)
        {
            if (item == null || item.itemData == null)
            {
                continue;
            }

            for (int i = 0; i < item.stackSize; i++)
            {
                dropManager.CreateItemDrop(item.itemData);
            }
        }
    }

    public void AddProgress(string questTargetID, int amount = 1)
    {
        List<QuestData> getRewardQuests = new List<QuestData>();

        foreach (var quest in activeQuests)
        {
            if (quest.questDataSO.questTargetID.ToString() != questTargetID)
            {
                continue;
            }

            quest.AddQuestProgress(amount);

            if (quest.questDataSO.rewardType == RewardType.None && quest.CanGetReward())
            {
                getRewardQuests.Add(quest);
            }
        }

        foreach (var quest in getRewardQuests)
        {
            GetQuestReward(quest.questDataSO);
            CompleteQuest(quest);

        }
    }

    public void AcceptQuest(QuestDataSO questDataSO)
    {
        QuestData newQuest = new QuestData(questDataSO);
        activeQuests.Add(newQuest);
    }

    public void CompleteQuest(QuestData questData)
    {
        QuestData questToComplete = activeQuests.Find(q => q == questData);
        if (questToComplete != null)
        {
            activeQuests.Remove(questToComplete);
            completedQuests.Add(questToComplete);
        }
    }

    public bool HasActiveQuest(QuestDataSO questToCheck)
    {
        if (questToCheck == null)
        {
            return false;
        }

        return activeQuests.Exists(q => q.questDataSO == questToCheck);
    }


    // SAVE AND LOAD SYSTEM
    public void LoadData(GameData data)
    {
        activeQuests.Clear();

        foreach (var quest in data.activeQuests)
        {
            string questSaveId = quest.Key;
            int progressAmount = quest.Value;

            QuestDataSO questDataSo = questDatabase.GetQuestByID(questSaveId);

            if (questDataSo == null)
            {
                Debug.LogWarning($"Quest with ID {questSaveId} not found in database.");
                continue;
            }

            QuestData questToLoad = new QuestData(questDataSo);
            questToLoad.currentAmount = progressAmount;

            activeQuests.Add(questToLoad);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.activeQuests.Clear();

        foreach (var quest in activeQuests)
        {
            data.activeQuests.Add(quest.questDataSO.questSaveID, quest.currentAmount);
        }

        foreach (var quest in completedQuests)
        {
            data.completedQuests.Add(quest.questDataSO.questSaveID, true);
        }
    }
}
