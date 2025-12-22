using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius;

    protected void DamageEnemiesInRadius(Transform transform, float radius)
    {
        foreach (var target in EnemiesAround(transform, radius))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
            {
                continue;
            }

            damageable.TakeDamage(1, 1, ElementType.None, transform); // Config later
        }
    }

    protected Collider2D[] EnemiesAround(Transform transform, float radius)
    {
        return Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);
    }

    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)
        {
            targetCheck = transform;
        }

        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }
}
