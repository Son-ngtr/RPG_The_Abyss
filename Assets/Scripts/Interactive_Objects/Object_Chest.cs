using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX fx => GetComponent<Entity_VFX>();
    private Entity_DropManager dropManager => GetComponent<Entity_DropManager>();

    [Header("Chest Settings")]
    [SerializeField] private Vector2 knockBack;
    [SerializeField] private bool canDropItems = true;

    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageSource)
    {
        if (canDropItems == false)
        {
            return false;
        }

        fx.PlayOnDamageVFX();
        anim.SetBool("chestOpen", true);
        rb.linearVelocity = knockBack;
        rb.angularVelocity = Random.Range(-200f, 200f);

        //Drop Items Here
        canDropItems = false; 
        dropManager?.DropItems();

        return true;
    }
}
