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

    public Vector2 movementInput { get; private set; }

    [Header("Collision Detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;
    public bool isGrounded { get; private set; }


    [Header("Movement Parameters")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Range(0, 1)]
    public float inAirMoveMultiplier = 0.7f;
    
    private bool isFacingRight = true;



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

    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        isFacingRight = !isFacingRight;
    }

    private void HandleCollisionDetection()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
    }

    private void OnDisable()
    {
        inputSet.Disable();
    }
}
