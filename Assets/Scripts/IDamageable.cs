using UnityEngine;

// Interface for objects that can take damage
public interface IDamageable 
{
    public void TakeDamage(float damage, Transform damageDealer);
}
