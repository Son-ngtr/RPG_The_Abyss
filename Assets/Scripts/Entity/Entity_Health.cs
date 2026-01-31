using System;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamageable
{
    // EVENT ACTION FOR UNIQUE ITEMS
    public event Action OnTakingDamage;
    public event Action OnHealthUpdate;

    private Slider healthBar;
    private Entity entity;
    private Entity_VFX entityVFX;
    private Entity_Stats entityStats;
    private Entity_DropManager dropManager;
    private Entity_SFX sfx;

    // HEALTH BAR
    private bool miniHealthBarActive ;

    [SerializeField] private float currentHealth;
    [Header("HEALTH REGEN")]
    [SerializeField] private float regenInterval = 1f;
    [SerializeField] private bool canRegenHealth = true;
    public float lastDamageTaken {  get; private set; }
    public bool isDead {  get; private set; }
    protected bool canTakeDamage = true;

    [Header("ON DAMAGE KNOCKBACK")]
    [SerializeField] private Vector2 knockbackForce = new Vector2 (1.5f, 2.5f);
    [SerializeField] private Vector2 heavyKnockbackForce = new Vector2(7f, 7f);
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float heavyKnockbackDuration = 0.5f;

    [Header("ON HEAVY DAMAGE")]
    [SerializeField] private float heavyDamageThreshold = 0.3f; // 30% of max health you should lose to be considered heavy damage

    protected virtual void Awake()
    {
        entityVFX = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();
        entityStats = GetComponent<Entity_Stats>();
        healthBar = GetComponentInChildren<Slider>();
        dropManager = GetComponent<Entity_DropManager>();
        sfx = GetComponent<Entity_SFX>();
    }

    protected virtual void Start()
    {
        SetupHealth();

    }

    private void SetupHealth()
    {
        if (entityStats == null)
        {
            return;
        }
        currentHealth = entityStats.GetMaxHealth();
        OnHealthUpdate += UpdateHealthBar;

        UpdateHealthBar();
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }

    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (isDead || canTakeDamage == false) return false;

        if (AttackEvaded())
        {
            Debug.Log($"{gameObject.name} evaded the attack!");
            return false;
        }

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0f;

        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0;
        float physicalDamageTaken = damage * (1 - mitigation);

        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0;
        float elementalDamageTaken = elementalDamage * (1 - resistance);

        TakeKnockBack(damageDealer, physicalDamageTaken);
        ReduceHeath(physicalDamageTaken + elementalDamageTaken);

        lastDamageTaken = physicalDamageTaken + elementalDamageTaken;


        OnTakingDamage?.Invoke();
        return true;
    }

    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    private bool AttackEvaded()
    {
        if (entityStats == null)
        {
            return false;
        }
        else
        {
            return UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();
        }
    }

    private void RegenerateHealth()
    {
        if (canRegenHealth == false)
        {
            return;
        }

        float regenAmount = entityStats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }

    public void IncreaseHealth(float healAmount)
    {
        if (isDead)
        {
            return;
        }

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();

        currentHealth = Mathf.Min(newHealth, maxHealth);
        OnHealthUpdate?.Invoke();
    }

    public void ReduceHeath(float damage)
    {
        entityVFX?.PlayOnDamageVFX();

        currentHealth -= damage;
        OnHealthUpdate?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public float GetHealthPercent() => currentHealth / entityStats.GetMaxHealth();

    public void SetHealthToPercent(float percent)
    {
        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp01(percent);
        OnHealthUpdate?.Invoke();
    }

    public float GetCurrentHealth() => currentHealth;

    private void UpdateHealthBar()
    {
        if (healthBar == null && healthBar.transform.parent.gameObject.activeSelf == false)
        {
            return;
        }
        healthBar.value = currentHealth/entityStats.GetMaxHealth();
    }

    public void EnableHealthBar(bool enable)
    {
        healthBar?.transform.parent.gameObject.SetActive(enable);
    }

    private void TakeKnockBack(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockBack(finalDamage, damageDealer);
        float duration = CalculateDuration(finalDamage);

        entity?.ReceiveKnockback(knockback, duration);
    }

    private Vector2 CalculateKnockBack(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;
        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackForce : knockbackForce;
        knockback.x *= direction;
        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;

    private bool IsHeavyDamage(float damage)
    {
        if (entityStats == null)
        {
            return false;
        }
        else
        {
            return damage / entityStats.GetMaxHealth() > heavyDamageThreshold;
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        sfx?.PlayDeathSFX();
        entity?.EntityDeath();

        dropManager?.DropItems();

    }
}
