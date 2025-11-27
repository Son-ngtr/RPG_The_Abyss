using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX vfx;
    public float damage = 25f;

    [Header("TARGET DETECTION")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;

    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
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
            bool targetGotHit = damageable.TakeDamage(damage, transform);

            if (targetGotHit)
            {
                vfx.CreateOnHitVfx(target.transform);   
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
