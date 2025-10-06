using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{

    private Entity entity;

    private Entity_Combat entityCombat;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
    }

    private void CurrentStateTrigger()
    {
        // access the Player component and call a method to signal that the attack is over
        entity.CurrentStateAnimationTrigger();
    }

    private void AttackTrigger()
    {
        Debug.Log("Attack Triggered");
        entityCombat.PerformAttack();
    }
}
