using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator { get; private set; }

    public Rigidbody2D rb { get; private set; }

    public Player_InputSet inputSet { get; private set; }
    private StateMachine stateMachine;

    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public PLayer_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }

    public Vector2 movementInput { get; private set; }

    [Header("COLLISION DETECTION")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;

    public bool isGrounded { get; private set; }
    public bool isTouchingWall { get; private set; }


    [Header("ATTACK DETAILS")]
    public Vector2[] attackVelocity; //3 1.5
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = 0.1f;
    public float comboResetTimer = 1f;
    private Coroutine queuedAttackCo;

    [Header("MOVEMENT PARAMETERS")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public Vector2 wallJumpForce = new Vector2 (6f, 12f);

    [Range(0, 1)]
    public float inAirMoveMultiplier = 0.7f;
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = 0.3f;
    [Space]
    public float dashDuration = 0.25f;
    public float dashSpeed = 20f;



    private bool isFacingRight = true;
    public int facingDirection { get; set; } = 1;



    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();
        inputSet = new Player_InputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new PLayer_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
    }

    private void OnEnable()
    {
        inputSet.Enable();

        // Subscribe to input events
        inputSet.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        // When movement is canceled, set movementInput to zero
        inputSet.Player.Movement.canceled += ctx => movementInput = Vector2.zero;
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateStateMachine();
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

    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (xVelocity < 0 && isFacingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        isFacingRight = !isFacingRight;
        facingDirection = facingDirection * -1;
    }

    private void HandleCollisionDetection()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isTouchingWall = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, groundLayer)
                        && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDirection, wallCheckDistance, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));
        Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDirection, 0));
    }

    private void OnDisable()
    {
        inputSet.Disable();
    }
}
