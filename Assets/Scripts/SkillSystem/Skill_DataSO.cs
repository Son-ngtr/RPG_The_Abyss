using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill data - ")]
public class Skill_DataSO : ScriptableObject
{
    [Header("SKILL DESCRIPTION")]
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("UNLOCK AND UNGRADESKILL")]
    public int cost;
    public bool unLockedByDefault;
    public SkillType skillType;
    public UpgradeData upgradeData;
}

[System.Serializable]
public class UpgradeData
{
    public SkillUpgradeType upgradeType;
    public float coolDown;
    public DamageScaleData damageScaleData;
}
