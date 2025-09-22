using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Update()
    {
        base.Update();
        // If there's no movement input, transition to idle state
        if (player.movementInput.x == 0f || player.isTouchingWall)
        {
            stateMachine.ChangeState(player.idleState);
        }

        player.SetVelocity(player.movementInput.x * player.moveSpeed, rb.linearVelocity.y);
    }
}
