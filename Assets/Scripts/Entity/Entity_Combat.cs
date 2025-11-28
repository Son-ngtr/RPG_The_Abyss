using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX vfx;
    private Entity_Stats stats;

    [Header("TARGET DETECTION")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("STATUS EFFECT DETAILS")]
    [SerializeField] private float defaultDuration = 3f;
    [SerializeField] private float chillSlowMultiplier = 0.5f;

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

            if (element != ElementType.None)
            {
                ApplyStatusEffect(target.transform, element);
            }

            if (targetGotHit)
            {
                vfx.UpdateOnHitVfxColor(element);
                vfx.CreateOnHitVfx(target.transform, isCritical);   
            }
        }
    }

    public void ApplyStatusEffect(Transform target, ElementType element, float scaleFactor = 1f)
    {
        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

        if (statusHandler == null)
        {
            return;
        }

        switch(element)
        {
            case ElementType.Fire:
                if (statusHandler.CanBeApplied(ElementType.Fire))
                {
                    float fireDamage = stats.offense.fireDamage.GetValue() * scaleFactor;
                    statusHandler.ApplyBurnEffect(defaultDuration, fireDamage);
                }
                break;
            case ElementType.Ice:
                if (statusHandler.CanBeApplied(ElementType.Ice))
                {
                    statusHandler.ApllyChillEffect(defaultDuration, chillSlowMultiplier * scaleFactor);
                }
                break;
            default:
                break;
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
