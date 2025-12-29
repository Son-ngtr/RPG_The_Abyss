using UnityEngine;

public class SkillObject_SwordPeirce : SkillObject_Sword
{
    private int amountToPierce;

    public override void SetupSword(Skill_SwordThrow manager, Vector2 direction)
    {
        base.SetupSword(manager, direction);

        amountToPierce = manager.pierceAmount;
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground");

        if (amountToPierce <= 0 || groundHit)
        {
            DamageEnemiesInRadius(transform, 0.3f);
            StopSword(collision);
            return;
        }

        amountToPierce--;
        DamageEnemiesInRadius(transform, 0.3f);
    }
}
