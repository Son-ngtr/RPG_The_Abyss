using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_Sword
{
    [SerializeField] private float bounceSpeed = 15f;
    private int bounceCount = 5;

    private Collider2D[] enemyTargets;
    private Transform nextTarget;
    private List<Transform> selectedBefore = new List<Transform>(); // store which targets has received damage


    public override void SetupSword(Skill_SwordThrow manager, Vector2 direction)
    {
        animator.SetTrigger("spin");
        base.SetupSword(manager, direction);

        bounceSpeed = manager.bounceSpeed;
        bounceCount = manager.bounceCount;
    }
    
    protected override void Update()
    {
        HandleComeback();
        HandleBounce();
    }


    private void HandleBounce()
    {
        if (nextTarget == null)
        {
            return;   
        }

        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextTarget.position) < 0.75f)
        {
            DamageEnemiesInRadius(transform, 1f);
            BounceToNextTarget();

            if (bounceCount == 0 || nextTarget == null)
            {
                nextTarget = null;
                GetSwordBacktoPlayer();
            }
        }
    }

    private void BounceToNextTarget()
    {
        nextTarget = GetNextTargets();
        bounceCount--;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyTargets == null)
        {
            enemyTargets = EnemiesAround(transform, 10f);
            rb.simulated = false;
        }

        DamageEnemiesInRadius(transform, 1f);

        if (enemyTargets.Length <= 1 || bounceCount == 0)
        {
            GetSwordBacktoPlayer();
        }
        else
        {
            nextTarget = GetNextTargets();
        }
    }


    private Transform GetNextTargets()
    {
        List<Transform> validTargets = GetValidTargets();

        int randomIndex = Random.Range(0, validTargets.Count);

        Transform nextTarget = validTargets[randomIndex];
        selectedBefore.Add(nextTarget);

        return nextTarget;
    }

    private List<Transform> GetValidTargets()
    {
        List<Transform> validTargets = new List<Transform>();
        List<Transform> aliveTargets = GetAliveTargets();

        foreach (var enemy in aliveTargets)
        {
            if (enemy != null && selectedBefore.Contains(enemy.transform) == false)
            {
                validTargets.Add(enemy.transform);
            }
        }

        if (validTargets.Count > 0)
        {
            return validTargets;
        }
        else
        {
            // When all enemy has been selected, refesh the selectedBefore list and start over
            selectedBefore.Clear();
            return aliveTargets;
        }
    }

    private List<Transform> GetAliveTargets()
    {
        List<Transform> aliveTargets = new List<Transform>();

        foreach (var enemy in enemyTargets)
        {
            if (enemy != null)
            {
                aliveTargets.Add(enemy.transform);
            }
        }

        return aliveTargets;
    }
}
