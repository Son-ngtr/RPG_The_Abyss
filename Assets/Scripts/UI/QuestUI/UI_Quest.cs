using UnityEngine;

public class UI_Quest : MonoBehaviour, ISaveable
{
    private GameData currentGameData;
    private UI_QuestSlot[] questSlots;

    [SerializeField] private UI_ItemSlotParent inventorySlots;
    [SerializeField] private UI_QuestPreview questPreview;

    public Player_QuestManager playerQuestManager;

    private void Awake()
    {
        questSlots = GetComponentsInChildren<UI_QuestSlot>(true);
        playerQuestManager = Player.instance.questManager;
    }


    public void SetupQuestUI(QuestDataSO[] questsToSetup)
    {
        foreach (var slot in questSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < questsToSetup.Length; i++)
        {
            questSlots[i].gameObject.SetActive(true);
            questSlots[i].SetupQuestSlot(questsToSetup[i]);
        }

        inventorySlots.UpdateSlots(Player.instance.inventory.itemList);
        questPreview.MakeQuestPreviewEmpty();

        UpdateQuestList();
    }

    public void UpdateQuestList()
    {
        foreach (var slot in questSlots)
        {
            if (slot.questInSlot == null)
            {
                continue;
            }

            if (slot.gameObject.activeSelf && CanTakeQuest(slot.questInSlot) == false)
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    private bool CanTakeQuest(QuestDataSO questToCheck)
    {
        bool questActive = playerQuestManager.HasActiveQuest(questToCheck);

        if (currentGameData != null) // Sometime, the game data is null (like in the beginning of the game)
        {
            bool questTaken = currentGameData.activeQuests.ContainsKey(questToCheck.questSaveID);
            bool questCompleted = currentGameData.completedQuests.TryGetValue(questToCheck.questSaveID, out bool isCompleted) && isCompleted;
            bool canDisplayQuest = questTaken == true || questCompleted == true;

            return questActive == false && canDisplayQuest == false;
        }

        return questActive == false;
    }

    public UI_QuestPreview GetQuestPreview()
    {
        return questPreview;
    }

    public void LoadData(GameData data)
    {
        currentGameData = data;
    }

    public void SaveData(ref GameData data)
    {
        
    }
}
