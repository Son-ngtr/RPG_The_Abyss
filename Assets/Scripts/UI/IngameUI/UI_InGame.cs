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
    // Scale yOffSetQuickItemParent theo CanvasScaler để hiển thị đúng tỉ lệ trên các độ phân giải
    [SerializeField] private bool scaleOffsetWithCanvas = true;

    // Display all options to select for a specific quick item slot
    private UI_QuickItemSlotSelectOption[] quickItemOptions;
    // Display the current quick item slots
    private UI_QuickItemSlot[] quickItemSlots;

    // Canvas để lấy scaleFactor (không thay đổi logic, chỉ cấu hình offset)
    private Canvas parentCanvas;

    // player reference setup call when awake so other scripts can use it in their start methods
    private void Start()
    {
        quickItemSlots = GetComponentsInChildren<UI_QuickItemSlot>();

        player = FindFirstObjectByType<Player>();
        player.health.OnHealthUpdate += UpdateHealthBar;

        inventory = player.inventory;
        inventory.OnInventoryChange += UpdateQuickSlotsUI;
        inventory.OnQuickSlotUsed += PlayQuickSlotFeedback;

        // Lấy Canvas để dùng scaleFactor cho offset (nếu có)
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void PlayQuickSlotFeedback(int slotNumber)
    {
        // slotNumber from event is 0-based (0 or 1), but UI slots use 1-based slotNumber (1 or 2)
        int slotIndex = slotNumber + 1;
        var slot = FindQuickItemSlotByNumber(slotIndex);
        if (slot != null)
        {
            slot.SimulateButtonFeedback();
        }
    }

    public void UpdateQuickSlotsUI()
    {
        Inventory_Item[] quickItems = inventory.quickItems;

        // Map inventory quickItems array (0-based) to UI slots (1-based slotNumber)
        for (int i = 0; i < quickItems.Length; i++)
        {
            int slotNumber = i + 1; // Convert to 1-based
            var slot = FindQuickItemSlotByNumber(slotNumber);
            if (slot != null)
            {
                slot.UpdateQuickSlotUI(quickItems[i]);
            }
        }
    }

    private UI_QuickItemSlot FindQuickItemSlotByNumber(int slotNumber)
    {
        if (quickItemSlots == null)
        {
            quickItemSlots = GetComponentsInChildren<UI_QuickItemSlot>();
        }

        foreach (var slot in quickItemSlots)
        {
            if (slot.SlotNumber == slotNumber)
            {
                return slot;
            }
        }

        return null;
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

        // Cấu hình: scale offset theo CanvasScaler nếu bật, không thay đổi logic sắp xếp hiển thị
        float scale = (scaleOffsetWithCanvas && parentCanvas != null) ? parentCanvas.scaleFactor : 1f;
        float actualYOffset = yOffSetQuickItemParent * scale;

        quickItemOptionsParent.position = targetRect.position + Vector3.up * actualYOffset;
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