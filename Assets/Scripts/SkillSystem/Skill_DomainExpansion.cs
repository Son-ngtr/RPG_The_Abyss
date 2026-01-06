using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{
    [SerializeField] private GameObject domainPrefab;

    [Header("SLOWING DOWN UPGRADE")]
    [SerializeField] private float slowdownPercent = 0.8f;
    [SerializeField] private float slowdownDomainDuration = 5f;

    [Header("SPELL CAST UPGRAFE")]
    [SerializeField] private int spellsToCast = 10;
    [SerializeField] private float spellCastingDomainDuration = 8f;
    [SerializeField] private float spellCastingDomainSlowdown = 1f;
    private float spellCastTimer;
    private float spellsPerSecond;

    [Header("DOMAIN DETAILS")]
    public float maxDomainSize = 10f;
    public float expendSpeed = 3f;

    private List<Enemy> trappedTargets = new List<Enemy>();
    private Transform currentTarget;



    public void CreateDomain()
    {
        spellsPerSecond = spellsToCast / GetDomainDuration();

        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<SkillObject_DomainExpansion>().SetupDomain(this);
    }

    public void DoSpellCasting()
    {
        spellCastTimer -= Time.deltaTime;

        if (currentTarget == null)
        {
            currentTarget = FindTargetInDomain();
        }

        if (currentTarget != null && spellCastTimer < 0)
        {
            CastSpell(currentTarget);
            spellCastTimer = 1 / spellsPerSecond;
            currentTarget = null; //Random target everytime cast spell
        }
    }

    private void CastSpell(Transform target)
    {
        if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
        {
            Vector3 offset = Random.value < 0.5f ? new Vector2(1,0) : new Vector2(-1,0);

            skillManager.timeEcho.CreateTimeEcho(target.position + offset);
        }

        if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
        {
            skillManager.shard.CreateRawShard(target, true);
        }
    }

    private Transform FindTargetInDomain()
    {
        if (trappedTargets.Count == 0)
        {
            return null;
        }

        int RandomIndex = Random.Range(0, trappedTargets.Count);
        Transform target = trappedTargets[RandomIndex].transform;

        if (target == null)
        {
            trappedTargets.RemoveAt(RandomIndex);
            return null;
        }

        return target;
    }

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

    public void AddTarget(Enemy targetToAdd)
    {
        trappedTargets.Add(targetToAdd);
    }

    public void ClearTargets()
    {
        foreach (var enemy in trappedTargets)
        {
            enemy.StopSlowDown();
        }

        trappedTargets.Clear();
    }
}
