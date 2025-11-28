using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private ElementType currentEffect = ElementType.None;

    private void Awake()
    {
        entityStats = GetComponent<Entity_Stats>();
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityHealth = GetComponent<Entity_Health>();
    }

    // CHILL EFFECT
    public void ApllyChillEffect(float duration, float slowMultiplier)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);
        float finalDuration = duration * (1 - iceResistance);

        StartCoroutine(ChilledEffectCo(finalDuration, slowMultiplier));
    }

    private IEnumerator ChilledEffectCo(float duration, float slowMultiplier)
    {
        currentEffect = ElementType.Ice;
        entity.SlowDownEntity(duration, slowMultiplier);
        entityVfx.PlayOnStatusVfx(duration, ElementType.Ice);

        yield return new WaitForSeconds(duration);
        currentEffect = ElementType.None;
    }


    // BURN EFFECT
    public void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);
        float finalDamage = fireDamage * (1 - fireResistance);

        StartCoroutine(BurnEffectCo(duration, finalDamage));
    }

    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire);

        int ticksPerSecond = 2;
        int tickCount = Mathf.RoundToInt(ticksPerSecond * duration);

        float damegePerTick = totalDamage / tickCount;
        float tickInterval = 1f / ticksPerSecond; // Remember to use float for accurate division, if just 1 / ticksPerSecond it would be integer division

        for (int i = 0; i < tickCount; i++)
        {
            // Reduce Health of Entity
            entityHealth.ReduceHp(damegePerTick);
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;
    }

    public bool CanBeApplied(ElementType element)
    {
        return currentEffect == ElementType.None;
    }
}
