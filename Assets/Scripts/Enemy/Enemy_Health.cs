using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;
    private Player_QuestManager playerQuestManager;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
        playerQuestManager = Player.instance.questManager;
    }


    public override bool TakeDamage(float damage,float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (canTakeDamage == false)
        {
            return false;
        }

        bool wasHit = base.TakeDamage(damage, elementalDamage, element, damageDealer);

        if (wasHit == false)
        {
            return false;
        }

        if (damageDealer.GetComponent<Player>() != null)
        {
            enemy.TryEnterBattleState(damageDealer);
        }

        return true;
    }

    protected override void Die()
    {
        base.Die();
        if (playerQuestManager == null)
        {
            return;
        }
        playerQuestManager.AddProgress(enemy.questTargetID.ToString());
    }
}
