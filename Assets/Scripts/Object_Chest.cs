using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX fx => GetComponent<Entity_VFX>();

    [Header("Chest Settings")]
    [SerializeField] private Vector2 knockBack;

    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageSource)
    {
        fx.PlayOnDamageVFX();
        anim.SetBool("chestOpen", true);
        rb.linearVelocity = knockBack;
        rb.angularVelocity = Random.Range(-200f, 200f);

        //Drop Items Here
        return true;
    }
}
