using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuickItemSlotSelectOption : UI_ItemSlot     
{
    private UI_QuickItemSlot currentQuickItemSlot;


    public void SetupOption(UI_QuickItemSlot currentQuickItemSlot, Inventory_Item itemToSet)
    {
        this.currentQuickItemSlot = currentQuickItemSlot;

        UpdateSlot(itemToSet);
    }

    // When clicking on the option, set the quick item slot to the selected item
    public override void OnPointerDown(PointerEventData eventData)
    {
        currentQuickItemSlot.SetupQuickSlotItem(itemInSlot);

        ui.ingameUI.HideQuickItemSelectOptions();
    }
}
