using UnityEngine;

public class Player_FallState : Player_AirState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Any specific logic when entering the fall state can be added here
    }

    public override void Update()
    {
        base.Update();
        if (player.isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
