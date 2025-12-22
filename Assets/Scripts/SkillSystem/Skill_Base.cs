using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    [Header("GENERAL DETAILS")]
    [SerializeField] protected SkillType skillType;
    [SerializeField] protected SkillUpgradeType upgradeType;

    [SerializeField] private float cooldown;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        lastTimeUsed = lastTimeUsed - cooldown;
    }

    public void SetSkillUpgrade(UpgradeData upgrade)
    {
        upgradeType = upgrade.upgradeType;
        cooldown = upgrade.coolDown;
    }

    public bool CanUseSkill()
    {
        if (OnCooldown())
        {
            Debug.Log("On Cooldown");
            return false;
        }

        // mana, unlock check in future

        return true;
    }

    protected bool Unlocked(SkillUpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;

    private bool OnCooldown() => Time.time < lastTimeUsed + cooldown;

    public void SetSkillOnCoolDown() => lastTimeUsed = Time.time;

    public void ResetCooldownBy(float cooldownReduction) => lastTimeUsed += cooldownReduction;

    public void ResetCooldown() => lastTimeUsed = Time.time;
}
