using UnityEngine;

public class Player_JumpState : Player_AirState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Apply jump force
        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        // Transition to fall state if the player is descending and not in jump attack state
        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
}
