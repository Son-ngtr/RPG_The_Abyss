using System;
using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{
    [SerializeField] private GameObject vfxPrefab;

    private Transform target;
    private float speed;

    public event Action OnExplore;
    private Skill_Shard shardManager;

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    } 

    public void MoveTowardsClosestTarget(float moveSpeed, Transform newTarget = null)
    {
        // Check if target is given or not
            // Given when using domain skill
            // Others will move to clostest target
        target = newTarget == null ? FindClosestTarget() : newTarget;
        this.speed = moveSpeed;
    }

    public void SetupShard(Skill_Shard shardManager)
    {
        this.shardManager = shardManager;

        playerStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;

        float detinationTime = shardManager.GetDetonateTime();

        Invoke(nameof(Explore), detinationTime);
    }

    public void SetupShard(Skill_Shard shardManager, float detinationTime, bool canMove, float shardSpeed, Transform target = null)
    {
        this.shardManager = shardManager;
        playerStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;

        Invoke(nameof(Explore), detinationTime);

        if (canMove)
        {
            MoveTowardsClosestTarget(shardSpeed, target);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
        {
            return;
        }

        Explore();
    }

    public void Explore()
    {
        DamageEnemiesInRadius(transform, checkRadius);
        GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = shardManager.player.vfx.GetElementColor(usedElement);

        OnExplore?.Invoke();
        Destroy(gameObject);
    }
}
