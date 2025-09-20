using UnityEngine;

public class Player : MonoBehaviour
{
    private Player_InputSet inputSet;
    private StateMachine stateMachine;

    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }

    public Vector2 movementInput { get; private set; }

    private void Awake()
    {
        stateMachine = new StateMachine();
        inputSet = new Player_InputSet();

        idleState = new Player_IdleState(this, stateMachine, "Idle");
        moveState = new Player_MoveState(this, stateMachine, "Move");
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
        stateMachine.UpdateStateMachine();
    }

    private void OnDisable()
    {
        inputSet.Disable();
    }
}
