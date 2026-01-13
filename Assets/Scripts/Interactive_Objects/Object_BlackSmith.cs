using UnityEngine;

public class Object_BlackSmith : Object_NPC, IInteractable
{
    private Animator animator;
    private Inventory_Player inventory;
    private Inventory_Storage storage;

    private bool switchBlackSmithUI = false;

    [Header("QUEST AND DIALOGUE")]
    [SerializeField] private DialogueLineSO firstDialogueLine;
    [SerializeField] private QuestDataSO[] quests;

    protected override void Awake()
    {
        base.Awake();

        storage = GetComponent<Inventory_Storage>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("isBlackSmith", true);
    }


    public override void Interact()
    {
        base.Interact();

        SwitchStorageAndCraftUI();
        if (switchBlackSmithUI == false)
        {
            ui.SwitchOffAllToolTips();
            ui.storageUI.gameObject.SetActive(false);
            ui.craftUI.gameObject.SetActive(false);
            return;
        }

        Debug.Log("Interacting with BlackSmith");
        ui.storageUI.SetupStorage(storage);
        ui.craftUI.SetupCraftUI(storage);

        ui.OpenDialogueUI(firstDialogueLine, new DialogueNpcData(npcRewardType, quests));
        //ui.OpenStorageUI(true);
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
        ui.OpenStorageUI(false);
    }

    private void SwitchStorageAndCraftUI()
    {
        switchBlackSmithUI = !switchBlackSmithUI;
    }
}
