using UnityEngine;

// Interface for objects that can take damage
public interface IDamageable 
{
    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer);
}
