using UnityEngine;

public class Player_WallSlideState : EntityState
{
    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Any specific logic when entering the wall slide state can be added here
    }

    public override void Update()
    {
        base.Update();
        HandleWallSlide();

        if (input.Player.Jump.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.wallJumpState);
        }

        if (!player.isTouchingWall)
        {
            stateMachine.ChangeState(player.fallState);
        }

        if (player.isGrounded)
        {
            stateMachine.ChangeState(player.idleState);

            /*if (player.facingDirection != player.movementInput.x)
            {
                player.Flip();

            }*/
        }
    }

    private void HandleWallSlide()
    {
        if (player.movementInput.y < 0)
        {
            player.SetVelocity(player.movementInput.x, rb.linearVelocity.y); // Slide down the wall slowly
        }
        else
        {
            player.SetVelocity(player.movementInput.x, rb.linearVelocity.y * player.wallSlideSlowMultiplier); // Stop vertical movement if not falling
        }
    }
}
