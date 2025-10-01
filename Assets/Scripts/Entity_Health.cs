using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    private Entity_VFX entityVFX;

    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected bool isDead;

    protected virtual void Awake()
    {
        entityVFX = GetComponent<Entity_VFX>();
    }

    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead) return;

        entityVFX?.PlayOnDamageVFX();
        ReduceHp(damage);
    }

    protected void ReduceHp(float damage)
    {
        maxHealth -= damage;
        if (maxHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
    }
}
