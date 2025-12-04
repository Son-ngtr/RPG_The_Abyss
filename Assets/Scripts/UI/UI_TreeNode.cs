using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rectTransform;
    private UI_SkillTree skillTree;

    [Header("UNLOCK DETAILS")]
    public UI_TreeNode[] neededNodes;
    public UI_TreeNode[] conflictNodes;
    public bool isUnlocked;
    public bool isLocked;

    [Header("SKILL DETAILS")]
    public Skill_DataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private int skillCost;
    [SerializeField] private string lockedColorHex = "#939393";
    private Color lastIconColor;

    private void OnValidate()
    {
        if (skillData == null)
        {
            return;
        }

        skillName = skillData.displayName;
        skillIcon.sprite = skillData.icon;
        skillCost = skillData.cost;
        gameObject.name = $"UI_TreeNode - {skillData.displayName}";
    }

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rectTransform = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();

        UpdateIconColor(GetColorByHex(lockedColorHex));
    }

    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        skillTree.RemoveSkillPoint(skillData.cost);
        LockConflictNodes();

        // Find player mng skill --> unlock skill
    }

    private bool canBeUnlocked()
    {
        if (isLocked || isUnlocked)
        {
            return false;
        }

        if (skillTree.EnoughSkillPoints(skillData.cost) == false)
        {
            return false;
        }

        foreach (var node in neededNodes)
        {
            if (node.isUnlocked == false)
            {
                return false;
            }
        }

        foreach (var node in conflictNodes)
        {
            if (node.isUnlocked)
            {
                return false;
            }
        }

        return true;
    }

    private void LockConflictNodes()
    {
        foreach (var node in conflictNodes)
        {
            node.isLocked = true;
            //node.UpdateIconColor(GetColorByHex(lockedColorHex));
        }
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
        ui.skillToolTip.ShowToolTip(true, rectTransform, this);

        if (isUnlocked == false)
        {
            UpdateIconColor(Color.white * 0.9f);   
        }
        Debug.Log("Pointer entered the UI_TreeNode.");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rectTransform);

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
