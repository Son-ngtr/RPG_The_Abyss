using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Player_QuestManager : MonoBehaviour
{
    public List<QuestData> activeQuests;

    public void AddProgress(string questTargetID, int amount = 1)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questDataSO.questTargetID.ToString() != questTargetID)
            {
                continue;
            }

            quest.AddQuestProgress(amount);
        }
    }

    public void AcceptQuest(QuestDataSO questDataSO)
    {
        QuestData newQuest = new QuestData(questDataSO);
        activeQuests.Add(newQuest);
    }

    public bool HasActiveQuest(QuestDataSO questToCheck)
    {
        if (questToCheck == null)
        {
            return false;
        }

        return activeQuests.Exists(q => q.questDataSO == questToCheck);
    }
}
