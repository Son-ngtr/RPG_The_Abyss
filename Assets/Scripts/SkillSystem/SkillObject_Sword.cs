using UnityEngine;


// THE SWORD can collider with ground and enemies
// But the deep or shallow it penetrates into an enemy depends on the force -- It bases on unity's physics engine 

public class SkillObject_Sword : SkillObject_Base
{
    protected Rigidbody2D rb;
    protected Skill_SwordThrow swordManager;

    protected Transform playerTransform;
    protected bool shouldComeback;
    protected float comebackSpeed = 20f;
    protected float maxAllowedDistance = 25f;

    private void Update()
    {
        transform.right = rb.linearVelocity;
        HandleComeback();
    }

    public virtual void SetupSword(Skill_SwordThrow manager, Vector2 direction)
    {
        rb = GetComponent<Rigidbody2D>();
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
