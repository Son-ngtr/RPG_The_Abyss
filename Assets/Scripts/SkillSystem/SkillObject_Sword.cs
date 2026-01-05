using UnityEngine;


// THE SWORD can collider with ground and enemies
// But the deep or shallow it penetrates into an enemy depends on the force -- It bases on unity's physics engine 

public class SkillObject_Sword : SkillObject_Base
{
    protected Skill_SwordThrow swordManager;

    protected Transform playerTransform;
    protected bool shouldComeback;
    protected float comebackSpeed = 20f;
    protected float maxAllowedDistance = 25f;

    protected virtual void Update()
    {
        transform.right = rb.linearVelocity;
        HandleComeback();
    }

    public virtual void SetupSword(Skill_SwordThrow manager, Vector2 direction)
    {
        rb.linearVelocity = direction;

        this.swordManager = manager;

        playerTransform = manager.transform.root;
        playerStats = manager.player.stats;
        damageScaleData = manager.damageScaleData;
    }

    public void GetSwordBacktoPlayer() => shouldComeback = true;

    protected void HandleComeback()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxAllowedDistance)
        {
            GetSwordBacktoPlayer();
        }

        if (shouldComeback == false)
        {
            return; 
        }

        rb.linearVelocity = Vector2.zero;     // Dừng velocity physics ngay lập tức
        rb.angularVelocity = 0f;
        // Tính hướng bay về player hiện tại
        Vector2 directionToPlayer = -1*(playerTransform.position - transform.position).normalized;
        if (directionToPlayer != Vector2.zero)
        {
            transform.right = directionToPlayer;
        }
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, comebackSpeed * Time.deltaTime);

        if (distanceToPlayer < 0.5f)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        StopSword(collision);

        DamageEnemiesInRadius(transform, 1f);
    }

    protected void StopSword(Collider2D collision)
    {
        rb.simulated = false;
        transform.parent = collision.transform;
    }
}
