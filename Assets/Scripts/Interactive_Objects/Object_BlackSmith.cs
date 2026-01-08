using UnityEngine;

public class Object_BlackSmith : Object_NPC, IInteractable
{
    private Animator animator;
    private Inventory_Player inventory;
    private Inventory_Storage storage;

    protected override void Awake()
    {
        base.Awake();

        storage = GetComponent<Inventory_Storage>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("isBlackSmith", true);
    }


    public void Interact()
    {
        Debug.Log("Interacting with BlackSmith");
        ui.storageUI.SetupStorage(inventory, storage);
        ui.storageUI.gameObject.SetActive(true);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // Get reference to the player inventory when the player enter interaction range
        inventory = player.GetComponent<Inventory_Player>();
        storage.SetInventory(inventory);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        ui.SwitchOffAllToolTips();
        ui.storageUI.gameObject.SetActive(false);
    }
}
