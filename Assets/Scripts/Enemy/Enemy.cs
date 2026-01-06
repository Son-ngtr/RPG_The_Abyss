 using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    public Enemy_Health health {  get; private set; }

    // States for all enemies to access easily and switch between them in their own scripts like Enemy_Skeleton.cs
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;
    public Enemy_StunnedState stunnedState;

    [Header("BATTLE DETAILS")]
    public float battleMoveSpeed = 3f;
    public float attackDistance = 2f;
    public float battleTimeDuration = 5f;
    public float minimumRetreatDistance = 1f;
    public Vector2 retreatVelocity;

    [Header ("MOVEMENT DETAILS" )]
    public float idleTime = 2f;
    public float moveSpeed = 1.4f;
    [Range(0f, 2f)]
    public float moveAnimSpeedMultiplier = 1f;

    [Header("STUNNED DETAILS")]
    public float stunnedDuration = 1f;
    public Vector2 stunnedVelocity = new Vector2 (7f, 7f);
    protected bool canBeStunned;

    [Header("PLAYER DETECTION")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10f;
    public Transform player { get; private set; }

    public float activeSlowMultiplier { get; private set; } = 1f;


    protected override void Awake()
    {
        base.Awake();

        health = GetComponent<Enemy_Health>();
    }

    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;
    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        activeSlowMultiplier = 1f - slowMultiplier;

        
        animator.speed *= activeSlowMultiplier;

        yield return new WaitForSeconds(duration);
        StopSlowDown();
    }

    public override void StopSlowDown()
    {
        activeSlowMultiplier = 1f;
        animator.speed = 1f;
        base.StopSlowDown();
    }

    public void EnableCounterWindow(bool enable) => canBeStunned = enable;

    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState || 
            stateMachine.currentState == attackState)
        {
            return;
        }
        this.player = player;
        stateMachine.ChangeState(battleState);
    }

    public Transform GetPlayerReference()
    {
        if (player == null)
        {
            player = PlayerDetected().transform;
        }
        return player;
    }

    public RaycastHit2D PlayerDetected()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDirection, playerCheckDistance, whatIsPlayer | groundLayer);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            return default;
        }
        return hit;
    }

    public override void EntityDeath()
    {
        base.EntityDeath();

        stateMachine.ChangeState(deadState);
    }

    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + facingDirection * playerCheckDistance, playerCheck.position.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(transform.position.x + facingDirection * attackDistance, transform.position.y));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + facingDirection * minimumRetreatDistance, transform.position.y));
    }
}
