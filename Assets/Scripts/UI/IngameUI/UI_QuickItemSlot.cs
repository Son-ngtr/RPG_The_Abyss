using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_QuickItemSlot : UI_ItemSlot
{
    private Button button;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private int slotNumber;
    
    public int SlotNumber => slotNumber;
    
    protected override void Awake()
    {
        base.Awake();
        button = GetComponent<Button>();
    }

    public void SetupQuickSlotItem(Inventory_Item itemToPass)
    {
        playerInventory.SetQuickItemsInSlot(slotNumber, itemToPass);
    }

    public void UpdateQuickSlotUI(Inventory_Item currentItemInSlot)
    {
        if (currentItemInSlot == null || currentItemInSlot.itemData == null)
        {
            itemIcon.sprite = defaultSprite;
            itemStackSize.text = "";
            return;
        }

        itemIcon.sprite = currentItemInSlot.itemData.itemIcon;
        itemStackSize.text = currentItemInSlot.stackSize.ToString();
    }


    // When clicking on the quick item slot, open the selection options
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.ingameUI.OpenQuickItemOptions(this, rectTransform);
    }

    // Simulate button feedback for controller/keyboard navigation
    public void SimulateButtonFeedback()
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);
        ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }
}
