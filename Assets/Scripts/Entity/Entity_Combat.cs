using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    // EVENT FOR UNIQUE ITEMS
    public event Action<float> OnDoingPhysicalDamage;

    private Entity_VFX vfx;
    private Entity_Stats stats;

    public DamageScaleData basicAttackScale; // All kind of attack scales can be defined here (physic, burn, chill, shock, etc.)

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


            AttackData attackData = stats.GetAttackData(basicAttackScale);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();


            float physicalDamage = attackData.physicalDamage;
            float elementalDamage = attackData.elementalDamage;
            ElementType element = attackData.element;

            bool targetGotHit = damageable.TakeDamage(physicalDamage, elementalDamage, element, transform);

            if (element != ElementType.None)
            {
                statusHandler?.ApplyStatusEffect(element, attackData.effectData); 
            }

            if (targetGotHit)
            {
                // Invoke event for unique items that react on dealing physical damage
                OnDoingPhysicalDamage?.Invoke(physicalDamage);

                vfx.CreateOnHitVfx(target.transform, attackData.isCrit, element);   
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
