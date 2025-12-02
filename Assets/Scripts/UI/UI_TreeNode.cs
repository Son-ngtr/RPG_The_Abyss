using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private Skill_DataSO skillData;
    [SerializeField] private string skillName;

    [SerializeField] private Image skillIcon;
    [SerializeField] private string lockedColorHex = "#939393";

    private Color lastIconColor;
    public bool isUnlocked;
    public bool isLocked;

    private void OnValidate()
    {
        if (skillData == null)
        {
            return;
        }

        skillName = skillData.displayName;
        skillIcon.sprite = skillData.icon;
        gameObject.name = $"UI_TreeNode - {skillData.displayName}";
    }

    private void Awake()
    {
        UpdateIconColor(GetColorByHex(lockedColorHex));
    }

    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);

        // Find player mng skill --> unlock skill
    }

    private bool canBeUnlocked()
    {
        if (isLocked || isUnlocked)
        {
            return false;
        }

        return true;
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
        {
            return;
        }
        
        lastIconColor = skillIcon.color;
        skillIcon.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canBeUnlocked())
        {
            Unlock();
            Debug.Log("Unlock skill");
        }
        else
        {
            Debug.Log("Cannot unlock skill");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUnlocked == false)
        {
            UpdateIconColor(Color.white * 0.9f);   
        }
        Debug.Log("Pointer entered the UI_TreeNode.");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isUnlocked == false)
        {
            UpdateIconColor(lastIconColor);            
        }
        Debug.Log("Pointer exited the UI_TreeNode.");
    }

    private Color GetColorByHex(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        return Color.white; // Default color if parsing fails
    }
}
