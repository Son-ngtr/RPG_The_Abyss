using UnityEngine;

public class Player_SwordThrowState : PlayerState
{
    private Camera mainCamera;

    public Player_SwordThrowState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillManager.swordThrow.EnableDots(true);

        if (mainCamera != Camera.main)
        {
            mainCamera = Camera.main;
        }
    }

    public override void Update()
    {
        base.Update();

        Vector2 directionToMouse = DirectionToMouse();

        player.SetVelocity(0, rb.linearVelocity.y);
        player.HandleFlip(directionToMouse.x);

        skillManager.swordThrow.PredictTrajectory(directionToMouse);


        if (input.Player.Attack.WasPressedThisFrame())
        {
            animator.SetBool("swordThrowPerformed", true);

            skillManager.swordThrow.EnableDots(false);
            skillManager.swordThrow.ConfirmTrajectory(directionToMouse);
        }

        if (input.Player.Attack.WasReleasedThisFrame() || triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    private Vector2 DirectionToMouse()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(player.mousePosition);
        
        Vector2 direction = mouseWorldPosition - playerPosition;

        return direction.normalized;
    }


    public override void Exit()
    {
        base.Exit();
        animator.SetBool("swordThrowPerformed", false);
        skillManager.swordThrow.EnableDots(false);
    }
}
