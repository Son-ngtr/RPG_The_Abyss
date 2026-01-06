using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (!enemy.isGrounded || enemy.isTouchingWall)
        {
            enemy.Flip();
        }
    }

    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.GetMoveSpeed() * enemy.facingDirection, rb.linearVelocity.y);

        if (!enemy.isGrounded || enemy.isTouchingWall)
        {
            stateMachine.ChangeState(enemy.idleState);            
        }
    }
}
