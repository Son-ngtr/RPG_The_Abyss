using UnityEngine;

public class Player_DomainExpansionState : PlayerState
{
    private Vector2 originalPosition;
    private float originalGravity;
    private float maxDistanceToGoUp;

    private bool isLevitating;
    private bool createdDomain;

    public Player_DomainExpansionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        originalPosition = player.transform.position;
        originalGravity = rb.gravityScale;
        maxDistanceToGoUp = GetAvailableRiseDistance();

        player.SetVelocity(0, player.riseSpeed);

    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(originalPosition, player.transform.position) >= maxDistanceToGoUp && isLevitating == false)
        {
            // Stop player, float in the air when reaching the distance
            Levitate();
        }

        if (isLevitating)
        {
            // Skill mnger cast spell
            skillManager.domainExpansion.DoSpellCasting();

            if (stateTimer < 0f)
            {
                rb.gravityScale = originalGravity;
                isLevitating = false;

                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    private void Levitate()
    {
        isLevitating = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        stateTimer = skillManager.domainExpansion.GetDomainDuration();
        // Get Levitation duration

        if (createdDomain == false)
        {
            createdDomain = true;
            // SKill mnger - create skill object domain
            skillManager.domainExpansion.CreateDomain();
        }
    }

    // Check anything above, anything prevent rising
    private float GetAvailableRiseDistance()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.up, player.riseMaxDistance, player.groundLayer);

        return hit.collider != null ? hit.distance - 1f : player.riseMaxDistance;
    }

    public override void Exit()
    {
        base.Exit();
 
        createdDomain = false;
    }
}
