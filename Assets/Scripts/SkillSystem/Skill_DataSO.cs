using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill data - ")]
public class Skill_DataSO : ScriptableObject
{
    public int cost;
    public SkillType skillType;
    public SkillUpgradeType upgradeType;

    [Header("SKILL DESCRIPTION")]
    public string displayName;
    [TextArea(4, 10)]
    public string description;
    public Sprite icon;
}
