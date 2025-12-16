using UnityEngine;

// PlayerState is Responsible for managing the different states of an entity
public abstract class PlayerState : EntityState
{
    protected Player player;
    protected Player_InputSet input;
    protected Player_SkillManager skills;
    
    // Constructor to initialize the state
    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        animator = player.animator;
        rb = player.rb;
        input = player.inputSet;
        stats = player.stats;
        skills = player.skillManager;
    }

    public override void Update()
    {
        base.Update();

        if (input.Player.Dash.WasCompletedThisFrame() && CanDash())
        {
            skills.dash.SetSkillOnCoolDown();
            stateMachine.ChangeState(player.dashState);
        }
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }
    
    private bool CanDash()
    {
        if (skills.dash.CanUseSkill() == false)
        {
            return false;
        }

        if (player.isTouchingWall)
        {
            return false;
        }

        if (stateMachine.currentState == player.dashState)
        {
            return false;
        }

        return true;
    }   
}
