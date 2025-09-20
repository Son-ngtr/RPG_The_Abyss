using UnityEngine;

// EntityState is Responsible for managing the different states of an entity
public abstract class EntityState
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string animBoolName;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected Player_InputSet input;

    // Constructor to initialize the state
    public EntityState(Player player, StateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;

        animator = player.animator;
        rb = player.rb;
        input = player.inputSet;
    }

    public virtual void Enter() 
    {
        // Every time state is entered, enter will be called
        animator.SetBool(animBoolName, true);
    }

    public virtual void Update() 
    {
        // Run the logic of the state
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public virtual void Exit() 
    {
        // Called when exiting the state and changeing to another state
        animator.SetBool(animBoolName, false);
    }
}
