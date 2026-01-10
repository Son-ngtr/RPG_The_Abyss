using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    private UI ui;
    private UI_SkillTree skillTree;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillCooldown;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    [Space]
    [SerializeField] private string metConditionHex;
    [SerializeField] private string unmetConditionHex;
    [SerializeField] private string importantInfoHex;
    [SerializeField] Color exampleColor;
    [SerializeField] private string lockedSkillText = "You've taken a different path - this skill is now locked.";

    private Coroutine textEffectCo;

    protected override void Awake()
    {
        base.Awake();
        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<UI_SkillTree>(true);
    }

    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    public void ShowToolTip(bool show, RectTransform targetRect, Skill_DataSO skillData, UI_TreeNode node)
    {
        base.ShowToolTip(show, targetRect);

        if (textEffectCo != null)
        {
            StopCoroutine(textEffectCo);
        }

        if (show == false)
        {
            return;
        }

        skillName.text = skillData.name;
        skillDescription.text = skillData.description;
        skillCooldown.text = "Cooldown: " + skillData.upgradeData.coolDown + " s.";

        if (node == null)
        {
            skillRequirements.text = "";
            return;
        }

        string skillLockedText = GetColoredText(importantInfoHex, lockedSkillText);
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.skillData.cost, node.neededNodes, node.conflictNodes);

        skillRequirements.text = requirements;
    }

    public void LockedSkillEffect()
    {
        StopLockSkillEffect();

        textEffectCo = StartCoroutine(TextBlinkEffectCo(skillRequirements, 0.15f, 3));
    }

    public void StopLockSkillEffect()
    {
        if (textEffectCo != null)
        {
            StopCoroutine(textEffectCo);
        }
    }

    private IEnumerator TextBlinkEffectCo(TextMeshProUGUI text, float blinkInterval, int blinkCount)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            text.text = GetColoredText(unmetConditionHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);

            text.text = GetColoredText(importantInfoHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private string GetRequirements(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Requirements:");
        string costColor = skillTree.EnoughSkillPoints(skillCost)
            ? metConditionHex
            : unmetConditionHex;
        string costText = $"- {skillCost} skill point(s)";
        string finalCostText = GetColoredText(costColor, costText);

        sb.AppendLine(finalCostText);

        foreach (var neededNode in neededNodes)
        {
            if (neededNode == null)
            {
                continue;
            }

            string nodeColor = neededNode.isUnlocked
                ? metConditionHex
                : unmetConditionHex;
            string nodeText = $"- {neededNode.skillData.displayName}";
            string finalNodeText = GetColoredText(nodeColor, nodeText);

            sb.AppendLine(finalNodeText);
        }

        if (conflictNodes.Length <= 0)
        {
            return sb.ToString();
        }

        sb.AppendLine();
        string locksOutText = $"Locks out: ";
        string finalLocksOutText = GetColoredText(importantInfoHex, locksOutText);
        sb.AppendLine(finalLocksOutText);

        foreach (var conflictNode in conflictNodes)
        {
            if (conflictNode == null)
            {
                continue;
            }

            string nodeText = $"- {conflictNode.skillData.displayName}";
            string finalNodeText = GetColoredText(importantInfoHex, nodeText);
            sb.AppendLine(finalNodeText);
        }
        return sb.ToString();
    }
}
