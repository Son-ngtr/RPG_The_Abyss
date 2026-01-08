using UnityEngine;

public class Object_BlackSmith : Object_NPC, IInteractable
{
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponentInChildren<Animator>();
        animator.SetBool("isBlackSmith", true);
    }


    public void Interact()
    {
        Debug.Log("Interacting with BlackSmith");
    }

    
}
