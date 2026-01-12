using UnityEngine;

public class Object_NPC : MonoBehaviour, IInteractable
{
    protected Transform player;
    protected UI ui;
    protected Player_QuestManager playerQuestManager;

    [Header("QUEST INFO")]
    [SerializeField] private QuestTargetID npcTargetQuestID;
    [SerializeField] private RewardType npcRewardType;

    [Space]
    [SerializeField] private Transform npc;
    [SerializeField] private GameObject interactToolTip;

    private bool facingRight = true;

    [Header("Floating Tooltip")]
    [SerializeField] private float floatSpeed = 8.0f;
    [SerializeField] private float floatRange = 0.25f;
    private Vector3 startPosition;

    protected virtual void Awake()
    {
        ui = FindFirstObjectByType<UI>();
        startPosition = interactToolTip.transform.position;
        interactToolTip.SetActive(false);
    }

    protected virtual void Start()
    {
        playerQuestManager = Player.instance.questManager; // Player created in Awake, so safe to access in Start
    }

    protected virtual void Update()
    {
        HandleNPCFlip();
        HandleToolTipFloat();
    }

    private void HandleToolTipFloat()
    {
        if (interactToolTip.activeSelf)
        {
            float yOffSet = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            interactToolTip.transform.position = startPosition + new Vector3(0, yOffSet, 0);
        }
    }

    private void HandleNPCFlip()
    {
        if (player == null || npc == null)
        {
            return;
        }

        if (npc.position.x > player.position.x && facingRight)
        {
            npc.transform.Rotate(0, 180, 0);
            facingRight = false;
        }
        else if (npc.position.x < player.position.x && !facingRight)
        {
            npc.transform.Rotate(0, 180, 0);
            facingRight = true;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Dont need to check for other tags since have configed in project settings to only detect player layer
        player = collision.transform;
        interactToolTip.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        interactToolTip.SetActive(false);
    }

    public virtual void Interact()
    {
        playerQuestManager.AddProgress(npcTargetQuestID.ToString());
        playerQuestManager.TryGetRewardFrom(npcRewardType);
    }
}
