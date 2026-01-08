using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public static event Action OnPlayerDeath;

    private UI ui;

    public Player_InputSet inputSet { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public Player_VFX vfx { get; private set; }
    public Entity_Health health { get; private set; }
    public Entity_StatusHandler statusHandler { get; private set; }
    public Player_Combat combat { get; private set; }


    #region State Variables

    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public PLayer_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }
    public Player_DeadState deadState { get; private set; }
    public Player_CounterAttackState counterAttackState { get; private set; }
    public Player_SwordThrowState swordThrowState { get; private set; }
    public Player_DomainExpansionState domainExpansionState { get; private set; }

    #endregion

    [Header("ATTACK DETAILS")]
    public Vector2[] attackVelocity; //3 1.5
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = 0.1f;
    public float comboResetTimer = 1f;
    private Coroutine queuedAttackCo;

    [Header("ULTIMATE ABILITY DETAILS")]
    public float riseSpeed = 25f;
    public float riseMaxDistance = 3f;

    [Header("MOVEMENT PARAMETERS")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public Vector2 wallJumpForce = new Vector2(6f, 12f);

    [Range(0, 1)]
    public float inAirMoveMultiplier = 0.7f;
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = 0.3f;
    [Space]
    public float dashDuration = 0.25f;
    public float dashSpeed = 20f;

    public Vector2 movementInput { get; private set; }
    public Vector2 mousePosition { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        inputSet = new Player_InputSet();

        ui = FindAnyObjectByType<UI>();
        vfx = GetComponent<Player_VFX>();
        health = GetComponent<Entity_Health>();
        skillManager = GetComponent<Player_SkillManager>();
        statusHandler = GetComponent<Entity_StatusHandler>();
        combat = GetComponent<Player_Combat>();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new PLayer_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
        deadState = new Player_DeadState(this, stateMachine, "dead");
        counterAttackState = new Player_CounterAttackState(this, stateMachine, "counterAttack");
        swordThrowState = new Player_SwordThrowState(this, stateMachine, "swordThrow");
        domainExpansionState = new Player_DomainExpansionState(this, stateMachine, "jumpFall");
    }

    private void OnEnable()
    {
        inputSet.Enable();

        // Capture mouse position
        inputSet.Player.Mouse.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
        // Capture movement input
        inputSet.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        // When movement is canceled, set movementInput to zero
        inputSet.Player.Movement.canceled += ctx => movementInput = Vector2.zero;

        // UI Inputs
        inputSet.Player.ToggleSkillTreeUI.performed += ctx => ui.ToggleSkillTreeUI();
        inputSet.Player.ToggleInventoryUI.performed += ctx => ui.ToggleInventoryUI();

        // Skill Inputs
        inputSet.Player.Spell.performed += ctx => skillManager.shard.TryUseSkill();
        inputSet.Player.Spell.performed += ctx => skillManager.timeEcho.TryUseSkill();

        // Interaction Input
        inputSet.Player.Interact.performed += ctx => TryInteract();

    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public void TeleportPlayer(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = animator.speed; 
        Vector2 originalWallJumpForce = wallJumpForce;
        Vector2 originalJumpAttack = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = new Vector2[attackVelocity.Length];
        Array.Copy(attackVelocity, originalAttackVelocity, attackVelocity.Length);

        float speedMultiplier = 1f - slowMultiplier;

        moveSpeed *= speedMultiplier;
        jumpForce *= speedMultiplier;
        animator.speed *= speedMultiplier;
        wallJumpForce *= speedMultiplier;
        jumpAttackVelocity *= speedMultiplier;
        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] *= speedMultiplier;
        }

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;
        animator.speed = originalAnimSpeed;
        wallJumpForce = originalWallJumpForce;
        jumpAttackVelocity = originalJumpAttack;
        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = originalAttackVelocity[i];
        }
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
        {
            StopCoroutine(queuedAttackCo);
        }
        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    // Cant not change state in the same frame as another state change
    // So we wait until end of frame to change to attack state
    // This prevents skipping animations when chaining attacks which can cause paused animations
    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    public override void EntityDeath()
    {
        base.EntityDeath();

        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(deadState);
    }


    private void TryInteract()
    {
        // Find closest interactable object within range and interact with it
        Transform clostest = null;
        float closestDistance = Mathf.Infinity;
        Collider2D[] objectsAround = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (var target in objectsAround)
        {
            IInteractable interactable = target.GetComponent<IInteractable>();
            if (interactable == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                clostest = target.transform;
            }

            if (clostest == null)
            {
                return;
            }

            clostest.GetComponent<IInteractable>().Interact();
        }
    }

    private void OnDisable()
    {
        inputSet.Disable();
    }
}
