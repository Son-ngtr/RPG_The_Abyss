using UnityEngine;

public class Enemy_AnimationTrigger : Entity_AnimationTriggers
{
    private Enemy enemy;
    private Enemy_VFX enemy_VFX;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
        enemy_VFX = GetComponentInParent<Enemy_VFX>();
    }

    private void EnableCounterWindow()
    {
        enemy.EnableCounterWindow(true);
        enemy_VFX.EnableAttackAlert(true);
    }

    private void DisableCounterWindow()
    {
        enemy.EnableCounterWindow(false);
        enemy_VFX.EnableAttackAlert(false);
    }
}

