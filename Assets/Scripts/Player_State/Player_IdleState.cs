using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) 
        : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Additional logic for entering idle state can be added here
        player.SetVelocity(0, rb.linearVelocity.y); // Stop horizontal movement
    }

    public override void Update()
    {
        base.Update();

        if (player.movementInput.x == player.facingDirection && player.isTouchingWall)
        {
            return;
        }

        if (player.movementInput.x != 0f)
        {
            stateMachine.ChangeState(player.moveState);
        }

    }
}
