using UnityEngine;

public class Enemy_DeadState : EnemyState
{
    private Collider2D enemyCollider;

    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyCollider = enemy.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        animator.enabled = false; // Disable animator to prevent any further animation changes
        enemyCollider.enabled = false;

        rb.gravityScale = 12f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15f);

        stateMachine.LockStateChanges(); // Prevent any further state changes

        enemy.DestroyGameObjectWithDelay(5f); // Destroy the enemy game object after 5 seconds
    }

}
