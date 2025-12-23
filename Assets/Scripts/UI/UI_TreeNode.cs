using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rectTransform;
    private UI_SkillTree skillTree;
    private UI_TreeConnectionHandler connectionHandler;

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
    private UnityEngine.Color lastIconColor;

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
        connectionHandler = GetComponent<UI_TreeConnectionHandler>();

        UpdateIconColor(GetColorByHex(lockedColorHex));

    }

    private void Start()
    {
        if (skillData.unLockedByDefault)
        {
            Unlock();
        }
        
    }

    public void Refund()
    {
        if (isUnlocked == true)
            skillTree.AddSkillPoints(skillData.cost);

        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));

        connectionHandler.UnlockConnectionImage(false);

        // skill mng and reset skill
    }

    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockConflictNodes();
        skillTree.RemoveSkillPoint(skillData.cost);
        connectionHandler.UnlockConnectionImage(true);

        // Find player mng skill --> unlock skill
        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData.upgradeData);
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
            node.LockChildNodes();
            //node.UpdateIconColor(GetColorByHex(lockedColorHex));
        }
    }

    public void LockChildNodes()
    {
        isLocked = true;

        foreach (var node in connectionHandler.GetChildNodes())
        {
            node.LockChildNodes();
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
        else if (isLocked)
        {
            ui.skillToolTip.LockedSkillEffect();
            Debug.Log("Cannot unlock skill");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rectTransform, this);

        if (isUnlocked || isLocked)
            return;
        
        ToggleNodeHighLight(true); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rectTransform);

        if (isUnlocked || isLocked)
            return;

        ToggleNodeHighLight(false);           
    }

    private void ToggleNodeHighLight(bool highlight)
    {
        Color highlightColor = Color.white * 0.9f; highlightColor.a = 1f;
        Color colorToApply = highlight ? highlightColor : lastIconColor;

        UpdateIconColor(colorToApply);
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

    private void OnDisable()
    {
        if (isLocked)
        {
            UpdateIconColor(GetColorByHex(lockedColorHex));
        }
        if (isUnlocked)
        {
            UpdateIconColor(Color.white);
        }
    }
}
