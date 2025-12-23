using System;
using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;

    [SerializeField] private GameObject shardPrefab;
    [SerializeField] private float detonationTime = 2f;

    [Header("Shard Upgrade Values")]
    [SerializeField] private float shardMoveSpeed = 3f;

    [Header("Multi-Shard Upgrade Values")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private int currentCharges;
    [SerializeField] private bool isRecharging;

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;
    }

    public void CreateShard()
    {
        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();
        currentShard.SetupShard(detonationTime);
    }

    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)
        {
            return;
        }

        if (Unlocked(SkillUpgradeType.Shard))
        {
            HandleShardRegular();
        }

        if (Unlocked(SkillUpgradeType.Shard_MoveToEnemy))
        {
            HandleShardMoving();
        }

        if (Unlocked(SkillUpgradeType.Shard_MultiCast))
        {
            HandleShardMulticast();
        }
    }

    public void HandleShardMulticast()
    {
        if (currentCharges <= 0)
        {
            return;
        }

        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardMoveSpeed);
        currentCharges--;

        if (isRecharging == false)
        {
            StartCoroutine(ShardRechargeCo());
        }
    }

    private IEnumerator ShardRechargeCo()
    {
        isRecharging = true;

        while (currentCharges < maxCharges)
        {
            yield return new WaitForSeconds(cooldown);
            currentCharges++;
        }

        isRecharging = false;
    }

    public void HandleShardMoving()
    {
        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardMoveSpeed);

        SetSkillOnCoolDown();
    }

    private void HandleShardRegular()
    {
        CreateShard();
        SetSkillOnCoolDown();
    }
}
