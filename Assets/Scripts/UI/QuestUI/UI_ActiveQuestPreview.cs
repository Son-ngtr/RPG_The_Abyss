using TMPro;
using UnityEngine;

public class UI_ActiveQuestPreview : MonoBehaviour
{
    private Player_QuestManager questManager;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI progress;
    [SerializeField] private UI_QuestRewardSlot[] questRewardSlots;


    public void SetupQuestPreview(QuestData questData)
    {
        questManager = Player.instance.questManager;
        QuestDataSO questDataSO = questData.questDataSO;

        questName.text = questDataSO.questName;
        questDescription.text = questDataSO.questDescription;

        progress.text = questDataSO.questGoal + " " + questManager.GetQuestProgress(questData) + "/" + questDataSO.requiredAmount;

        foreach (var slot in questRewardSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < questDataSO.rewardItems.Length; i++)
        {
            questRewardSlots[i].gameObject.SetActive(true);
            questRewardSlots[i].UpdateSlot(questDataSO.rewardItems[i]);
        }
    }
}
