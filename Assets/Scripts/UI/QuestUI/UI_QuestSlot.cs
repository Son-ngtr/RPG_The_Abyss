using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image[] rewardsQuickPreviewSlots; // Slots to show - Take from UI_QuestRewardSlot

    public QuestDataSO questInSlot { get; private set; }
    private UI_QuestPreview questPreview;

    public void SetupQuestSlot(QuestDataSO questDataSO)
    {
        questPreview = transform.root.GetComponentInChildren<UI_Quest>().GetQuestPreview();

        questInSlot = questDataSO;
        questName.text = questInSlot.questName;

        foreach (var previewIcon in rewardsQuickPreviewSlots)
        {
            previewIcon.gameObject.SetActive(false);
        }

        for (int i = 0; i < questInSlot.rewardItems.Length; i++)
        {
            if (questDataSO.rewardItems[i] == null && questDataSO.rewardItems[i].itemData == null)
            {
                continue;
            }

            Image slot = rewardsQuickPreviewSlots[i];

            slot.gameObject.SetActive(true);
            slot.sprite = questInSlot.rewardItems[i].itemData.itemIcon;
            slot.GetComponentInChildren<TextMeshProUGUI>().text = questInSlot.rewardItems[i].stackSize.ToString();
        }
    }

    public void UpdateQuestPreview()
    {
        Debug.Log("Updating quest preview UI");
        questPreview.SetupQuestPreview(questInSlot);
    }
}
