using UnityEngine;

public class Player_Health : Entity_Health
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Die();
        }
    }

    protected override void Die()
    {
        base.Die();
        
        // Trigger death ui
        //GameManager.instance.SetLastPlayerPosition(transform.position);

        // OPEN DEATH UI

        GameManager.instance.RestartScene();
    }
}
