using UnityEngine;

// PlayerState is Responsible for managing the different states of an entity
public abstract class PlayerState : EntityState
{
    protected Player player;
    protected Player_InputSet input;
    protected Player_SkillManager skillManager;
    
    // Constructor to initialize the state
    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        animator = player.animator;
        rb = player.rb;
        input = player.inputSet;
        stats = player.stats;
        skillManager = player.skillManager;
    }

    public override void Update()
    {
        base.Update();

        if (input.Player.Dash.WasCompletedThisFrame() && CanDash())
        {
            skillManager.dash.SetSkillOnCoolDown();
            stateMachine.ChangeState(player.dashState);
        }

        if (input.Player.UltimateSpell.WasPressedThisFrame() && skillManager.domainExpansion.CanUseSkill())
        {
            if (skillManager.domainExpansion.InstantDomain())
            {
                skillManager.domainExpansion.CreateDomain();
            }
            else
            {
                stateMachine.ChangeState(player.domainExpansionState);
            }

            skillManager.domainExpansion.SetSkillOnCoolDown();
        }
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }
    
    private bool CanDash()
    {
        if (skillManager.dash.CanUseSkill() == false)
        {
            return false;
        }

        if (player.isTouchingWall)
        {
            return false;
        }

        if (stateMachine.currentState == player.dashState || stateMachine.currentState == player.domainExpansionState)
        {
            return false;
        }

        return true;
    }   
}
