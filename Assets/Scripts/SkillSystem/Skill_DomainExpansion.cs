using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{
    [SerializeField] private GameObject domainPrefab;

    [Header("SLOWING DOWN UPGRADE")]
    [SerializeField] private float slowdownPercent = 0.8f;
    [SerializeField] private float slowdownDomainDuration = 5f;

    [Header("SPELL CAST UPGRAFE")]
    [SerializeField] private float spellCastingDomainDuration = 8f;
    [SerializeField] private float spellCastingDomainSlowdown = 1f;

    [Header("DOMAIN DETAILS")]
    public float maxDomainSize = 10f;
    public float expendSpeed = 3f;

    public float GetDomainDuration()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
        {
            return slowdownDomainDuration;
        }
        else
        {
            return spellCastingDomainDuration;
        }
    }

    public float GetSlowPercentage()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
        {
            return slowdownPercent;
        }
        else
        {
            return spellCastingDomainSlowdown;
        }
    }

    // Defy just cast spell or enter state then cast spell
    public bool InstantDomain()
    {
        return upgradeType != SkillUpgradeType.Domain_EchoSpam
            && upgradeType != SkillUpgradeType.Domain_ShardSpam;
    }


    public void CreateDomain()
    {
        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<SkillObject_DomainExpansion>().SetupDomain(this);
    }
}
