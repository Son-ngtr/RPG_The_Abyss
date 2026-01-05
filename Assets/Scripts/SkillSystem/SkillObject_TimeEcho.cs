using UnityEngine;

public class SkillObject_TimeEcho : SkillObject_Base
{
    [SerializeField] private GameObject onDeathVfx;
    [SerializeField] private LayerMask whatIsGround;
    private Skill_TimeEcho echoManager;

    public void SetupEcho(Skill_TimeEcho manager)
    {
        echoManager = manager;


        Invoke(nameof(HandleDeath), echoManager.GetEchoDuration());
    }

    private void Update()
    {
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        StopHorizontalMovement();
    }

    public void HandleDeath()
    {
        Instantiate(onDeathVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void StopHorizontalMovement()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, whatIsGround);

        if (hit.collider != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }
}
