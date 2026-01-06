using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{

    // Defy just cast spell or enter state then cast spell
    public bool InstantDomain()
    {
        return upgradeType != SkillUpgradeType.Domain_EchoSpam
            && upgradeType != SkillUpgradeType.Domain_ShardSpam;
    }


    public void CreateDomain()
    {
        Debug.Log("create skill object");
    }
}
