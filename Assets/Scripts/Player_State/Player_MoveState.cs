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
        if (player.movementInput == Vector2.zero)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else
        {
            // Handle movement logic here
            Vector3 moveDirection = new Vector3(player.movementInput.x, 0, player.movementInput.y);
            player.transform.Translate(moveDirection * Time.deltaTime * 5f, Space.World);
        }

        player.SetVelocity(player.movementInput.x * player.moveSpeed, rb.linearVelocity.y);
    }
}
