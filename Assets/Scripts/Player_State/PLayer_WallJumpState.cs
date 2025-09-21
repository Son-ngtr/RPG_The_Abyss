using UnityEngine;

public class PLayer_WallJumpState : EntityState
{
    public PLayer_WallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.SetVelocity(player.wallJumpForce.x * -player.facingDirection, player.wallJumpForce.y);
    }

    public override void Update()
    {
        base.Update();
        if (rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
        }

        if (player.isTouchingWall)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
