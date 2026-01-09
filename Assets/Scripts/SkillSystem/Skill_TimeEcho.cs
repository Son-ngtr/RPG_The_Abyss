using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab;
    [SerializeField] private float timeEchoDuration;

    [Header("Attack Upgrades")]
    [SerializeField] private int maxAttacks = 3;
    [SerializeField] private float duplicateChance = 0.3f;

    [Header("HEAL WISP UPGRADES")]
    [SerializeField] private float damagePercentHealed = 0.3f;
    [SerializeField] private float cooldownReducedInSeconds;

    public float GetPercentOfDamageHealed()
    {
        if (ShouldBeWisp() == false)
        {
            return 0;
        }

        return damagePercentHealed;
    }

    public float GetCooldownReducedInSeconds()
    {
        if (upgradeType != SkillUpgradeType.TimeEcho_Cooldownwisp)
        {
            return 0;
        }

        return cooldownReducedInSeconds;
    }

    public bool CanRemoveNegativeEffects()
    {
        return upgradeType == SkillUpgradeType.TimeEcho_Cleansewisp;
    }

    public bool ShouldBeWisp()
    {
        return upgradeType == SkillUpgradeType.TimeEcho_Healwisp 
            || upgradeType == SkillUpgradeType.TimeEcho_Cleansewisp 
            || upgradeType == SkillUpgradeType.TimeEcho_Cooldownwisp;
    }

    public float GetDuplicateChance()
    {
        if (upgradeType != SkillUpgradeType.TimeEcho_ChangeToDuplicate)
        {
            return 0;
        }

        return duplicateChance;
    }

    public int GetMaxAttacks()
    {
        if (upgradeType == SkillUpgradeType.TimeEcho_SingleAttack || upgradeType == SkillUpgradeType.TimeEcho_ChangeToDuplicate)
        {
            return 1;
        }

        if (upgradeType == SkillUpgradeType.TimeEcho_MultiAttack)
        {
            return maxAttacks;
        }

        return 0;
    }

    public float GetEchoDuration()
    {
        return timeEchoDuration;
    }

    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)
        {
            return;
        }

        CreateTimeEcho();
        SetSkillOnCoolDown();
    }

    public void CreateTimeEcho(Vector3? targetPosition = null)
    {
        Vector3 position = targetPosition ?? transform.position;

        GameObject timeEcho = Instantiate(timeEchoPrefab, position, Quaternion.identity);

        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this);
    }
}
