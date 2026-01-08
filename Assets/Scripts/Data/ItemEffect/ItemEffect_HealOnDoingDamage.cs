using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Heal on doing damage", fileName = "Item effect data - Heal on doing damage")]

public class ItemEffect_HealOnDoingDamage : ItemEffect_DataSO
{
    [SerializeField] private float percentHealedOnAttack = 0.2f;


    public override void SubScribeToPlayerEvents(Player player)
    {
        base.SubScribeToPlayerEvents(player);

        player.combat.OnDoingPhysicalDamage += HealOnDoingDamage;
    }

    public override void UnSubScribeToPlayerEvents(Player player)
    {
        base.UnSubScribeToPlayerEvents(player);

        player.combat.OnDoingPhysicalDamage -= HealOnDoingDamage;
        player = null;
    }

    private void HealOnDoingDamage(float damageDealt)
    {
        player.health.IncreaseHealth(damageDealt * percentHealedOnAttack);
    }
}
