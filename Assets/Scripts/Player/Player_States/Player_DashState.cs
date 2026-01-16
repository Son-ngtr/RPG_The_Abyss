    using UnityEngine;

public class Player_DashState : PlayerState
{
    private float originalGravityScale;
    private int dashDirection;

    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillManager.dash.OnStartEffect();
        player.vfx.DoImageEchoEffect(player.dashDuration);

        dashDirection = player.movementInput.x != 0 ? (int)player.movementInput.x : player.facingDirection; ;
        stateTimer = player.dashDuration;

        originalGravityScale = player.rb.gravityScale;
        rb.gravityScale = 0f;

        player.health.SetCanTakeDamage(false);
        player.gameObject.layer = LayerMask.NameToLayer("Untargetable");
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(player.dashSpeed * dashDirection, 0f);

        if (stateTimer < 0)
        {
            if (player.isGrounded)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.fallState);
            }
        }

        CancleDashIfNeeded();
    }

    private void CancleDashIfNeeded()
    {
        if (player.isTouchingWall)
        {
            if (player.isGrounded)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
        }
        
    }

    public override void Exit()
    {
        base.Exit();

        skillManager.dash.OnEndEffect();

        player.health.SetCanTakeDamage(true);

        player.SetVelocity(0f, 0f);
        rb.gravityScale = originalGravityScale;
        player.gameObject.layer = LayerMask.NameToLayer("Player");
    }
}
