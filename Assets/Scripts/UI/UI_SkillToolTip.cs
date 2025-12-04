using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    private UI_SkillTree skillTree;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    [Space]
    [SerializeField] private string metConditionHex;
    [SerializeField] private string unmetConditionHex;
    [SerializeField] private string importantInfoHex;
    [SerializeField] Color exampleColor;
    [SerializeField] private string lockedSkillText = "You've taken a different path - this skill is now locked.";

    protected override void Awake()
    {
        base.Awake();
        skillTree = GetComponentInParent<UI_SkillTree>();
    }

    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    public void ShowToolTip(bool show, RectTransform targetRect, UI_TreeNode node)
    {
        base.ShowToolTip(show, targetRect);

        if (show == false)
        {
            return;
        }

        skillName.text = node.skillData.name;
        skillDescription.text = node.skillData.description;

        string skillLockedText = $"<color={importantInfoHex}>{lockedSkillText}</color>";
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.skillData.cost, node.neededNodes, node.conflictNodes);

        skillRequirements.text = requirements;
    }

    private string GetRequirements(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Requirements:");
        string costColor = skillTree.EnoughSkillPoints(skillCost)
            ? metConditionHex
            : unmetConditionHex;
        sb.AppendLine($" - <color={costColor}>{skillCost} skill point(s)</color>");
        foreach (var neededNode in neededNodes)
        {
            string nodeColor = neededNode.isUnlocked
                ? metConditionHex
                : unmetConditionHex;
            sb.AppendLine($" - <color={nodeColor}>{neededNode.skillData.displayName}</color>");
        }

        if (conflictNodes.Length <= 0)
        {
            return sb.ToString();
        }

        sb.AppendLine();
        sb.AppendLine($"<color={importantInfoHex}>Locks out: </color>");
        foreach (var conflictNode in conflictNodes)
        {
            sb.AppendLine($" - <color={importantInfoHex}>{conflictNode.skillData.displayName}</color>");
        }
        return sb.ToString();
    }
}
