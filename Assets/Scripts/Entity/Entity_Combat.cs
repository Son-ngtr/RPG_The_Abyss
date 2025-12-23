using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    private Entity_VFX vfx;
    private Entity_Stats stats;

    public DamageScaleData basicAttackScale;

    [Header("TARGET DETECTION")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("STATUS EFFECT DETAILS")]
    [SerializeField] private float defaultDuration = 3f;
    [SerializeField] private float chillSlowMultiplier = 0.5f;
    [SerializeField] private float electrifyChargeBuildup = 0.4f;
    [Space]
    [SerializeField] private float fireScale = 0.8f;
    [SerializeField] private float lightningScale = 2.5f;

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

            ElementalEffectData effectData = new ElementalEffectData(stats, basicAttackScale);

            float elementalDamage = stats.GetElementalDamage(out ElementType element, 0.6f);

            bool isCritical; // This will be set by GetPhysicalDamage method
            float damage = stats.GetPhysicalDamage(out isCritical);
            bool targetGotHit = damageable.TakeDamage(damage, elementalDamage, element, transform);

            if (element != ElementType.None)
            {
                target.GetComponent<Entity_StatusHandler>().ApplyStatusEffect(element, effectData); 
            }

            if (targetGotHit)
            {
                vfx.UpdateOnHitVfxColor(element);
                vfx.CreateOnHitVfx(target.transform, isCritical);   
            }
        }
    }

    /*public void ApplyStatusEffect(Transform target, ElementType element, float scaleFactor = 1f)
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
                    scaleFactor = fireScale;
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
            case ElementType.Lightning:
                if (statusHandler.CanBeApplied(ElementType.Lightning))
                {
                    scaleFactor = lightningScale;
                    float lightingDamage = stats.offense.lightningDamage.GetValue() * scaleFactor;
                    statusHandler.ApplyLightningEffect(defaultDuration, lightingDamage, electrifyChargeBuildup);
                }
                break;
            default:
                break;
        }
    }*/

    public Collider2D[] GetDetectedColliders()
    {
       return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
