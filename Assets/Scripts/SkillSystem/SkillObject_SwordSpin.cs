using UnityEngine;

public class SkillObject_SwordSpin : SkillObject_Sword
{
    private int maxDistance;
    private float attackPerSecond;
    private float attackTimer;

    public override void SetupSword(Skill_SwordThrow manager, Vector2 direction)
    {
        base.SetupSword(manager, direction);

        animator?.SetTrigger("spin");

        this.maxDistance = manager.maxDistance;
        this.attackPerSecond = manager.attackPerSecond;

        Invoke(nameof(GetSwordBacktoPlayer), manager.maxSpinDuration);
    }

    protected override void Update()
    {
        HandleAttack();
        HandleStopping();
        HandleComeback();
    }

    private void HandleStopping()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceFromPlayer > maxDistance && rb.simulated == true)
        {
            rb.simulated = false;
        }
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer < 0)
        {
            DamageEnemiesInRadius(transform, 1);
            attackTimer = 1f / attackPerSecond;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        rb.simulated = false;
    }
}
