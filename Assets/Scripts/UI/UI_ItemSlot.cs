using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory_Item itemInSlot {  get; private set; }
    protected Inventory_Player playerInventory;
    protected UI ui;
    protected RectTransform rectTransform;

    [Header("UI SLOT SETUP")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemStackSize;


    private void Awake()
    {
        playerInventory = FindFirstObjectByType<Inventory_Player>();
        ui = GetComponentInParent<UI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null || itemInSlot.itemData.itemType == ItemType.Material)
        {
            return;
        }

        if (itemInSlot.itemData.itemType == ItemType.Comsumable)
        {
            if (itemInSlot.itemEffect.CanBeUsed() == false)
            {
                return;
            }

            playerInventory.TryUseItem(itemInSlot);
        }
        else
        {
            playerInventory.TryEquipItem(itemInSlot);
        }


        if (itemInSlot == null)
        {
            ui.itemToolTip.ShowToolTip(false, null);
        }
    }

    public void UpdateSlot(Inventory_Item item)
    {
        itemInSlot = item;

        if (itemInSlot == null)
        {
            itemStackSize.text = "";
            itemIcon.sprite = null;
            itemIcon.color = Color.clear;
            return;
        }

        Color color = Color.white; color.a = 0.9f;
        itemIcon.color = color;
        itemIcon.sprite = itemInSlot.itemData.itemIcon;
        itemStackSize.text = item.stackSize > 1 ? item.stackSize.ToString() : "";
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null)
        {
            return;
        }

        ui.itemToolTip.ShowToolTip(true, rectTransform, itemInSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemToolTip.ShowToolTip(false, null);
    }
}
