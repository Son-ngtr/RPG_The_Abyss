using UnityEngine;

public class Enemy_Skeleton : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        deadState = new Enemy_DeadState(this, stateMachine, "idle"); // Dead state can use any animation since it won't be changed anymore
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
}
