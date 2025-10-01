using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    public float damage = 50f;

    [Header("TARGET DETECTION")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;

    public void PerformAttack()
    {
        foreach (var target in GetDetectedColliders())
        {
            Debug.Log("Hit " + target.name);
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            targetHealth?.TakeDamage(damage, transform);
        }
    }

    private Collider2D[] GetDetectedColliders()
    {
       return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
