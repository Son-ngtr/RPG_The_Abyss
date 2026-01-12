using TMPro;
using UnityEngine;

public class UI_QuestPreview : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI questGoal;

    [SerializeField] private UI_QuestRewardSlot[] rewardSlots;
    [SerializeField] private GameObject[] addditionalObjects; // Objects to enable/disable when preview is empty/full

    private UI_Quest questUI;
    private QuestDataSO previousQuest;

    public void SetupQuestPreview(QuestDataSO questDataSO)
    {
        questUI = transform.root.GetComponentInChildren<UI_Quest>();
        previousQuest = questDataSO;

        EnableAdditionalObjects(true);
        EnableRewardSlots(false);

        questName.text = questDataSO.questName;
        questDescription.text = questDataSO.questDescription;
        questGoal.text = questDataSO.questGoal + " " + questDataSO.requiredAmount;

        for (int i = 0; i < questDataSO.rewardItems.Length; i++)
        {
            Inventory_Item rewardItem = new Inventory_Item(questDataSO.rewardItems[i].itemData);
            rewardItem.stackSize = questDataSO.rewardItems[i].stackSize;

            rewardSlots[i].gameObject.SetActive(true);
            rewardSlots[i].UpdateSlot(rewardItem);
        }
    }

    public void AcceptQuestButton()
    {
        MakeQuestPreviewEmpty();

        questUI.playerQuestManager.AcceptQuest(previousQuest);
        questUI.UpdateQuestList();
    }

    public void MakeQuestPreviewEmpty()
    {
        questName.text = string.Empty;
        questDescription.text = string.Empty;

        EnableAdditionalObjects(false);
        EnableRewardSlots(false);
    }

    private void EnableAdditionalObjects(bool enable)
    {
        foreach (var obj in addditionalObjects)
        {
            obj.SetActive(enable);
        }
    }

    private void EnableRewardSlots(bool enable)
    {
        foreach (var slot in rewardSlots)
        {
            slot.gameObject.SetActive(enable);
        }
    }
}
