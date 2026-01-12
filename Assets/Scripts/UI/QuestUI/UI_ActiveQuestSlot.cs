using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ActiveQuestSlot : MonoBehaviour
{
    private QuestData questDataInSlot;
    private UI_ActiveQuestPreview questPreview;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image[] questRewardsPreview;


    // MOTIPO: Setup questslots to display by taking the data, clear the current ui and fill it with new data
    public void SetupActiveQuestSlot(QuestData questDataToSetup)
    {
        questPreview = transform.root.GetComponentInChildren<UI_ActiveQuestPreview>();
        questDataInSlot = questDataToSetup;

        questName.text = questDataInSlot.questDataSO.questName;

        Inventory_Item[] rewards = questDataToSetup.questDataSO.rewardItems;

        foreach (var previewIcon in questRewardsPreview)
        {
             previewIcon.gameObject.SetActive(false);
        }

        for (int i = 0; i < rewards.Length; i++)
        {
            if (rewards[i] == null)
            {
                continue;
            }

            Image previewIcon = questRewardsPreview[i];
            previewIcon.gameObject.SetActive(true);
            previewIcon.sprite = rewards[i].itemData.itemIcon;
            previewIcon.GetComponentInChildren<TextMeshProUGUI>().text = rewards[i].stackSize.ToString();
        }
    }

    public void SetupPreviewButton()
    {
        questPreview.SetupQuestPreview(questDataInSlot);
    }
}
