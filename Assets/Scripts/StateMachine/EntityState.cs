using UnityEngine;
using UnityEngine.Windows;

public abstract class EntityState
{
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator animator;
    protected Rigidbody2D rb;

    protected float stateTimer;
    protected bool triggerCalled;

    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        // Every time state is entered, enter will be called
        animator.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;      
        UpdateAnimationParameters();
    }

    public void AnimationTrigger()
    {
        triggerCalled = true;
    }

    public virtual void Exit()
    {
        // Called when exiting the state and changeing to another state
        animator.SetBool(animBoolName, false);
    }

    public virtual void UpdateAnimationParameters()
    {
        // Used to update any parameters in the animator that are not handled by the state machine
    }
}
