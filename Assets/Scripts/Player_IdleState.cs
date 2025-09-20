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

        if (player.movementInput != Vector2.zero)
        {
            stateMachine.ChangeState(player.moveState);
        }

        if (player.inputSet.Player.Jump.WasPerformedThisFrame())
        {
            // Transition to jump state if implemented
            // stateMachine.ChangeState(player.jumpState);
            Debug.Log("Jump input detected, but jump state not implemented.");
        }
    }
}
