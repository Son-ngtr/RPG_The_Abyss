using UnityEngine;

public class Entity_Health : MonoBehaviour, IDamageable
{
    private Entity_VFX entityVFX;
    private Entity entity;

    [SerializeField] private float currentHp;
    [SerializeField] protected float maxHp = 100f;
    [SerializeField] protected bool isDead;

    [Header("ON DAMAGE KNOCKBACK")]
    [SerializeField] private Vector2 knockbackForce = new Vector2 (1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackForce = new Vector2(7f, 7f);
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float heavyKnockbackDuration = 0.5f;

    [Header("ON HEAVY DAMAGE")]
    [SerializeField] private float heavyDamageThreshold = 0.3f; // 30% of max health you should lose to be considered heavy damage

    protected virtual void Awake()
    {
        entityVFX = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();

        currentHp = maxHp;
    }

    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead) return;

        Vector2 knockback = CalculateKnockBack(damage, damageDealer);
        float duration = CalculateDuration(damage);
        entity?.ReceiveKnockback(knockback, duration);

        entityVFX?.PlayOnDamageVFX();
        ReduceHp(damage);
    }

    protected void ReduceHp(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private Vector2 CalculateKnockBack(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackForce : knockbackForce;
        knockback.x *= direction;
        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;

    private bool IsHeavyDamage(float damage) => damage / maxHp > heavyDamageThreshold;

    private void Die()
    {
        isDead = true;
        entity.EntityDeath();
    }
}
