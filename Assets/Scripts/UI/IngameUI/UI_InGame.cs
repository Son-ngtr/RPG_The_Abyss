using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private Player player;
    private Inventory_Player inventory;
    private UI_SkillSlot[] skillSlots;

    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("QUICK ITEM SLOTS")]
    [SerializeField] private float yOffSetQuickItemParent = 200f;
    [SerializeField] private Transform quickItemOptionsParent;
    // Display all options to select for a specific quick item slot
    private UI_QuickItemSlotSelectOption[] quickItemOptions;
    // Display the current quick item slots
    private UI_QuickItemSlot[] quickItemSlots;


    // player reference setup call when awake so other scripts can use it in their start methods
    private void Start()
    {
        quickItemSlots = GetComponentsInChildren<UI_QuickItemSlot>();

        player = FindFirstObjectByType<Player>();
        player.health.OnHealthUpdate += UpdateHealthBar;

        inventory = player.inventory;
        inventory.OnInventoryChange += UpdateQuickSlotsUI;
        inventory.OnQuickSlotUsed += PlayQuickSlotFeedback;
    }


    public void PlayQuickSlotFeedback(int slotNumber)
    {
        quickItemSlots[slotNumber].SimulateButtonFeedback();
    }

    public void UpdateQuickSlotsUI()
    {
        Inventory_Item[] quickItems = inventory.quickItems;

        for (int i = 0; i < quickItems.Length; i++)
        {
            quickItemSlots[i].UpdateQuickSlotUI(quickItems[i]);
        }
    }


    public void OpenQuickItemOptions(UI_QuickItemSlot quickItemSlot, RectTransform targetRect)
    {
        if (quickItemOptions == null)
        {
            quickItemOptions = quickItemOptionsParent.GetComponentsInChildren<UI_QuickItemSlotSelectOption>(true);
        }

        List<Inventory_Item> comsumableItems = inventory.itemList.FindAll(item => item.itemData.itemType == ItemType.Comsumable);

        for (int i = 0; i < quickItemOptions.Length; i++)
        {
            if (i < comsumableItems.Count)
            {
                quickItemOptions[i].gameObject.SetActive(true);
                quickItemOptions[i].SetupOption(quickItemSlot, comsumableItems[i]);
            }
            else
            {
                quickItemOptions[i].gameObject.SetActive(false);
            }
        }

        quickItemOptionsParent.position = targetRect.position + Vector3.up * yOffSetQuickItemParent;
    }

    public void HideQuickItemSelectOptions()
    {
        quickItemOptionsParent.position = new Vector3(0, 9999, 0);
    }

    public UI_SkillSlot GetSkillSlot(SkillType skillType)
    {
        if (skillSlots == null)
        {
            skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
        }

        foreach (var slot in skillSlots)
        {
            if (slot.skillType == skillType)
            {
                slot.gameObject.SetActive(true);
                return slot;
            }
        }

        return null;
    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.health.GetCurrentHealth());
        float maxHealth = Mathf.RoundToInt(player.stats.GetMaxHealth());
        float sizeDifference = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);

        if (sizeDifference > 0.1f)
        {
            healthRect.sizeDelta = new Vector2(maxHealth, healthRect.sizeDelta.y);
        }

        healthText.text = currentHealth + "/" + maxHealth;
        healthSlider.value = player.health.GetHealthPercent();
    }
}
