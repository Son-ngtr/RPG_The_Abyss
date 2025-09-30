using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{

    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    private void CurrentStateTrigger()
    {
        // access the Player component and call a method to signal that the attack is over
        entity.CurrentStateAnimationTrigger();
    }
}
