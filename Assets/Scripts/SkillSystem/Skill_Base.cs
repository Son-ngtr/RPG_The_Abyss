using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    public Player_SkillManager skillManager { get; private set; }

    public Player player;

    public DamageScaleData damageScaleData { get; private set; }

    [Header("GENERAL DETAILS")]
    [SerializeField] protected SkillType skillType;
    [SerializeField] protected SkillUpgradeType upgradeType;

    [SerializeField] protected float cooldown;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        skillManager = GetComponentInParent<Player_SkillManager>();
        player = GetComponentInParent<Player>();
        lastTimeUsed = lastTimeUsed - cooldown;

        damageScaleData = new DamageScaleData();
    }

    public virtual void TryUseSkill()
    {
        
    }

    public void SetSkillUpgrade(UpgradeData upgrade)
    {
        upgradeType = upgrade.upgradeType;
        cooldown = upgrade.coolDown;

        damageScaleData = upgrade.damageScaleData;
    }

    public virtual bool CanUseSkill()
    {
        if (upgradeType == SkillUpgradeType.None)
        {
            return false;
        }

        if (OnCooldown())
        {
            Debug.Log("On Cooldown");
            return false;
        }

        // mana, unlock check in future

        return true;
    }

    protected bool Unlocked(SkillUpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;

    protected bool OnCooldown() => Time.time < lastTimeUsed + cooldown;

    public void SetSkillOnCoolDown() => lastTimeUsed = Time.time;

    public void ResetCooldownBy(float cooldownReduction) => lastTimeUsed += cooldownReduction;

    public void ResetCooldown() => lastTimeUsed = Time.time;
}
