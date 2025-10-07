using UnityEngine;

public class Player_CounterAttackState : PlayerState
{
    private Player_Combat combat;
    private bool counteredSomebody;

    public Player_CounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }

    public override void Enter()
    {
        base.Enter();
        counteredSomebody = false;
        animator.SetBool("counterAttackPerformed", false);

        stateTimer = combat.GetCounterDuration();
    }

    public override void Update()
    {
        base.Update();

        if (combat.CounterAttackPerformed())
        {
            counteredSomebody = true;
            animator.SetBool("counterAttackPerformed", true);
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (stateTimer < 0 && counteredSomebody == false)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
