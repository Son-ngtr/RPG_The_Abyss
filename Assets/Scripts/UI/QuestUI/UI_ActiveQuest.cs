using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActiveQuest : MonoBehaviour
{
    private Player_QuestManager playerQuestManager;
    private UI_ActiveQuestSlot[] activeQuestSlots;
    private void Awake()
    {
        playerQuestManager = Player.instance.questManager;
        activeQuestSlots = GetComponentsInChildren<UI_ActiveQuestSlot>(true);
    }

    private void OnEnable()
    {
        List<QuestData> quests = playerQuestManager.activeQuests;

        foreach (var slot in activeQuestSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < quests.Count; i++)
        {
            activeQuestSlots[i].gameObject.SetActive(true);
            activeQuestSlots[i].SetupActiveQuestSlot(quests[i]);
        }

        if (quests.Count > 0)
        {
            activeQuestSlots[0].SetupPreviewButton();
        }
    }
 
}
