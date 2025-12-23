using System;
using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    private Entity_Health playerHealth;

    [SerializeField] private GameObject shardPrefab;
    [SerializeField] private float detonationTime = 2f;

    [Header("SHARD SKILL")]
    [SerializeField] private float shardMoveSpeed = 3f;

    [Header("MULTICAST SHARD SKILL")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private int currentCharges;
    [SerializeField] private bool isRecharging;

    [Header("TELEPORT SHARD SKILL")]
    [SerializeField] private float shardExistDuration = 10f;

    [Header("HEALTH REWIND SHARD SKILL")]
    [SerializeField] private float savedHealthPercent;

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;
        playerHealth = GetComponentInParent<Entity_Health>();
    }

    public void CreateShard()
    {
        float currentDetonationTime = GetDetonateTime();

        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();
        currentShard.SetupShard(currentDetonationTime);

        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        {
            currentShard.OnExplore += ForceCoolDown;
        }
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

        if (Unlocked(SkillUpgradeType.Shard_Teleport))
        {
            HandleShardTeleport();
        }

        if (Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        {
            HandleShardHealthRewind();
        }
    }

    private void HandleShardHealthRewind()
    {
        if (currentShard == null)
        {
            CreateShard();
            savedHealthPercent = playerHealth.GetHealthPercent();
        }
        else
        {
            SwapPlayerAndShard();
            playerHealth.SetHealthToPercent(savedHealthPercent);
            SetSkillOnCoolDown();
        }
    }

    private void HandleShardTeleport()
    {
        if (currentShard == null)
        {
            CreateShard();
        }
        else
        {
            SwapPlayerAndShard();
            SetSkillOnCoolDown();
        }
    }

    private void SwapPlayerAndShard()
    {
        Vector3 shardPosition = currentShard.transform.position;
        Vector3 playerPosition = player.transform.position;

        currentShard.transform.position = playerPosition;
        currentShard.Explore();

        player.TeleportPlayer(shardPosition);
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

    public float GetDetonateTime()
    {
        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        {
            return shardExistDuration;
        }

        return detonationTime;
    }

    private void ForceCoolDown()
    {
        if (OnCooldown() == false)
        {
            SetSkillOnCoolDown();
            currentShard.OnExplore -= ForceCoolDown;
        }
    }
}
