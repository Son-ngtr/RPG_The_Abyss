using UnityEngine;

public class SkillObject_AnimationTrigger : MonoBehaviour
{
    private SkillObject_TimeEcho timeEcho;

    private void Awake()
    {
        timeEcho = GetComponentInParent<SkillObject_TimeEcho>();
    }


    public void AttackTrigger()
    {
        timeEcho.PerformAttack();
    }

    // Call in animator controller --> use for configure how many atks can uses
    private void TryTerminate(int currentAttackIndex)
    {
        if (currentAttackIndex == timeEcho.maxAttacks)
        {
            timeEcho.HandleDeath();
        }
    }
}
