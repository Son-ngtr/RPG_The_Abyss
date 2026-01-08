using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Ice Blast", fileName = "Item effect data - Ice Blast On Taking DMG")]

public class ItemEffect_IceBlastOnTakingDamage : ItemEffect_DataSO
{
    [SerializeField] private ElementalEffectData elementEffectData;
    [SerializeField] private float iceDamage;
    [SerializeField] private LayerMask whatIsEnemy;

    [Space]
    [SerializeField] private float healthPercentTrigger = 0.25f; // Trigger when HP is reduced by this percent
    [SerializeField] private float cooldown;
    private float lastTriggerTime = -999;

    [Header("Ice Blast Effect Settings")]
    [SerializeField] private GameObject iceBlastVfx;
    [SerializeField] private GameObject onHitVfx;

    public override void ExecuteEffect()
    {
        // Ice Blast effect logic to be implemented
            // When HP is reduced, trigger an ice blast effect that damages and slows nearby enemies.
        bool noCoolDown = Time.time >= lastTriggerTime + cooldown;
        bool reachedThreshold = player.health.GetHealthPercent() <= healthPercentTrigger;

        if (noCoolDown && reachedThreshold)
        {
            //VFX
            player.vfx.CreateEffectOf(iceBlastVfx, player.transform);
            lastTriggerTime = Time.time;

            //Damage Enemies
            DamageEnemiesWithIce();
        }
    }

    private void DamageEnemiesWithIce()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, 1.5f, whatIsEnemy);

        foreach (var target in enemies)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
            {
                continue;
            }

            bool targetGotHit = damageable.TakeDamage(0, iceDamage, ElementType.Ice, player.transform);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
            statusHandler?.ApplyStatusEffect(ElementType.Ice, elementEffectData);

            if (targetGotHit)
            {
                player.vfx.CreateEffectOf(onHitVfx, target.transform);
            }
        }
    }

    public override void SubScribeToPlayerEvents(Player player)
    {
        base.SubScribeToPlayerEvents(player);

        // Subscribe to player's damage event
        // player.OnTakeDamage += TriggerIceBlast;
        player.health.OnTakingDamage += ExecuteEffect;
    }

    public override void UnSubScribeToPlayerEvents(Player player)
    {
        base.UnSubScribeToPlayerEvents(player);

        // Unsubscribe from player's damage event
        // player.OnTakeDamage -= TriggerIceBlast;
        player.health.OnTakingDamage -= ExecuteEffect;
        player = null;
    }
}
