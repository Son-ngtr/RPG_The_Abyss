using System;
using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{
    [SerializeField] private GameObject vfxPrefab;

    private Transform target;
    private float speed;

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    } 

    public void MoveTowardsClosestTarget(float moveSpeed)
    {
        target = FindClosestTarget();
        this.speed = moveSpeed;
    }

    public void SetupShard(float detinationTime)
    {
        Invoke(nameof(Explore), detinationTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
        {
            return;
        }

        Explore();
    }

    private void Explore()
    {
        DamageEnemiesInRadius(transform, checkRadius);
        Instantiate(vfxPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
