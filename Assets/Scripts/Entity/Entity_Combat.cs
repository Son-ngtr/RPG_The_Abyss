using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX vfx;
    private Entity_Stats stats;

    [Header("TARGET DETECTION")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();
    }

    public void PerformAttack()
    {
        foreach (var target in GetDetectedColliders())
        {
            // Try to get IDamageable component from the target
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null)
            {
                continue;
            }

            float elementalDamage = stats.GetElementalDamage(out ElementType element);

            bool isCritical; // This will be set by GetPhysicalDamage method
            float damage = stats.GetPhysicalDamage(out isCritical);
            bool targetGotHit = damageable.TakeDamage(damage, elementalDamage, element, transform);

            if (targetGotHit)
            {
                vfx.CreateOnHitVfx(target.transform, isCritical);   
            }
        }
    }

    public Collider2D[] GetDetectedColliders()
    {
       return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
