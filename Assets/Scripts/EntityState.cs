using UnityEngine;

// EntityState is Responsible for managing the different states of an entity
public abstract class EntityState
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string stateName;

    // Constructor to initialize the state
    public EntityState(Player player, StateMachine stateMachine, string stateName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.stateName = stateName;
    }

    public virtual void Enter() 
    {
        // Every time state is entered, enter will be called
        Debug.Log("Entering state: " + stateName);
    }

    public virtual void Update() 
    {
        // Run the logic of the state
        Debug.Log("Updating state: " + stateName);
    }

    public virtual void Exit() 
    {
        // Called when exiting the state and changeing to another state
        Debug.Log("Exiting state: " + stateName);
    }
}
