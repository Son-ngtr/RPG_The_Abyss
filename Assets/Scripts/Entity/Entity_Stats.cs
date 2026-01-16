using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaultStatSetup;

    public Stat_MajorGroup major;
    public Stat_ResourceGroup resources;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defend;


    protected virtual void Awake()
    {

    }


    public void AdjustStatSetup(Stat_ResourceGroup resourceGroup, Stat_OffenseGroup offenseGroup, Stat_DefenseGroup defenseGroup, float penalty, float increase)
    {
        // INCREASE STATS
        offense.damage.SetBaseValue(offenseGroup.damage.GetValue() * increase);
        offense.attackSpeed.SetBaseValue(offenseGroup.attackSpeed.GetValue() * increase);
        offense.critChance.SetBaseValue(offenseGroup.critChance.GetValue() * increase);
        offense.critDamage.SetBaseValue(offenseGroup.critDamage.GetValue() * increase);
        offense.fireDamage.SetBaseValue(offenseGroup.fireDamage.GetValue() * increase);
        offense.iceDamage.SetBaseValue(offenseGroup.iceDamage.GetValue() * increase);
        offense.lightningDamage.SetBaseValue(offenseGroup.lightningDamage.GetValue() * increase);

        defend.evasion.SetBaseValue(defenseGroup.evasion.GetValue() * increase);

        // PENALTY STATS
        resources.maxHealth.SetBaseValue(resourceGroup.maxHealth.GetValue() * penalty);
        resources.healthRegen.SetBaseValue(resourceGroup.healthRegen.GetValue() * penalty);

        defend.armor.SetBaseValue(defenseGroup.armor.GetValue() * penalty);
        defend.lightningResist.SetBaseValue(defenseGroup.lightningResist.GetValue() * penalty);
        defend.fireResist.SetBaseValue(defenseGroup.fireResist.GetValue() * penalty);
        defend.iceResist.SetBaseValue(defenseGroup.iceResist.GetValue() * penalty);
    }


    public AttackData GetAttackData(DamageScaleData scaleData)
    {
        return new AttackData(this, scaleData);
    }

    public float GetElementalDamage(out ElementType element, float scaleFactor = 1f)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue() * 1f; // Each point in intelligence gives 1 elemental damage

        float highestElementalDamage = fireDamage;
        element = ElementType.Fire;

        if (iceDamage > highestElementalDamage)
        {
            highestElementalDamage = iceDamage;
            element = ElementType.Ice;
        }

        if (lightningDamage > highestElementalDamage)
        {
            highestElementalDamage = lightningDamage;
            element = ElementType.Lightning;
        }

        if (highestElementalDamage <= 0)
        {
            element = ElementType.None;
            return 0;
        }

        float bonusFire = (element == ElementType.Fire) ? 0 : fireDamage * 0.5f;
        float bonusIce = (element == ElementType.Ice) ? 0 : iceDamage * 0.5f;
        float bonusLightning = (element == ElementType.Lightning) ? 0 : lightningDamage * 0.5f;

        float weakerElementalDamage = bonusFire + bonusIce + bonusLightning;
        float finalElementalDamage = highestElementalDamage + weakerElementalDamage + bonusElementalDamage;

        return finalElementalDamage * scaleFactor;
    }

    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * 0.5f; // Bonus res from intelligence gives 0.5% per INT

        switch (element)
        {
            case ElementType.Fire:
                baseResistance = defend.fireResist.GetValue();
                break;
            case ElementType.Ice:
                baseResistance = defend.iceResist.GetValue();
                break;
            case ElementType.Lightning:
                baseResistance = defend.lightningResist.GetValue();
                break;
        }

        float resistance = baseResistance + bonusResistance;
        float resistanceCap = 75f;

        float finalResistance = Mathf.Clamp(resistance, 0, resistanceCap) / 100;

        return finalResistance;
    }

    public float GetPhysicalDamage(out bool isCriticalHit, float scaleFactor = 1f)
    {
        float baseDamage = GetBaseDamage();
        float critChance = GetCritChance();
        float critDamage = GetCritDamage() / 100;

        isCriticalHit = Random.Range(1, 100) < critChance;
        float finalDamage = isCriticalHit ? baseDamage * critDamage : baseDamage;

        return finalDamage * scaleFactor;
    }

    // Each point in strength gives 1 damage 
    public float GetBaseDamage() => offense.damage.GetValue() + major.strength.GetValue() * 1f;
    // Each point in agility gives 0.3% crit chance
    public float GetCritChance() => offense.critChance.GetValue() + major.agility.GetValue() * 0.3f;
    // Each point in strength gives 0.5% crit damage
    public float GetCritDamage() => offense.critDamage.GetValue() + major.strength.GetValue() * 0.5f; 

    public float GetArmorReduction()
    {
        float finalReduction = offense.armorReduction.GetValue() / 100;

        return finalReduction;
    }

    public float GetArmorMitigation(float armorReduction)
    {
        float totalArmor = GetBaseArmor();

        float reductionMultiplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100f); // Diminishing returns formula
        float mitigationCap = 0.85f; // Maximum mitigation percentage

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);
        return finalMitigation;
    }

    public float GetBaseArmor() => defend.armor.GetValue() + major.vitality.GetValue() * 1f; // Each point in vitality gives 1 armor

    public float GetMaxHealth()
    {
        float baseMaxHealth = resources.maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5;

        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;

        return finalMaxHealth;
    }

    public float GetEvasion()
    {
        float baseAvasion = defend.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 0.5f; // Each point in agility gives 0.5% evasion

        float totalEvation = baseAvasion + bonusEvasion;
        float evasionCap = 85f; // Maximum evasion percentage

        float finalEvasion = Mathf.Clamp(totalEvation, 0, evasionCap);

        return finalEvasion;
    }

    public Stat GetStatByType(StatType statType)
    {
        return statType switch
        {
            // Resources
            StatType.MaxHealth => resources.maxHealth,
            StatType.HealthRegen => resources.healthRegen,
            // Major Stats
            StatType.Strength => major.strength,
            StatType.Agility => major.agility,
            StatType.Intelligence => major.intelligence,
            StatType.Vitality => major.vitality,
            // Offense Stats
            StatType.AttackSpeed => offense.attackSpeed,
            StatType.Damage => offense.damage,
            StatType.CritChance => offense.critChance,
            StatType.CritPower => offense.critDamage,
            StatType.ArmorReduction => offense.armorReduction,

            StatType.FireDamage => offense.fireDamage,
            StatType.IceDamage => offense.iceDamage,
            StatType.LightningDamage => offense.lightningDamage,
            // Defense Stats
            StatType.Armor => defend.armor,
            StatType.Evasion => defend.evasion,
            StatType.FireResistance => defend.fireResist,
            StatType.IceResistance => defend.iceResist,
            StatType.LightningResistance => defend.lightningResist,
            _ => null,
        };
    }

    [ContextMenu("Update Default Stats Setup")]
    public void ApplyDefaultStatsSetup()
    {
        if (defaultStatSetup == null)
        {
            Debug.Log("No default stat setup assigned");
            return;
        }

        // Major Stats
        major.strength.SetBaseValue(defaultStatSetup.strength);
        major.agility.SetBaseValue(defaultStatSetup.agility);
        major.intelligence.SetBaseValue(defaultStatSetup.intelligence);
        major.vitality.SetBaseValue(defaultStatSetup.vitality);

        // Resources
        resources.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);

        // Offense - Physical Damage
        offense.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        offense.damage.SetBaseValue(defaultStatSetup.damage);
        offense.critChance.SetBaseValue(defaultStatSetup.critChance);
        offense.critDamage.SetBaseValue(defaultStatSetup.critDamage);
        offense.armorReduction.SetBaseValue(defaultStatSetup.armorReduction);

        // Offense - Elemental Damage
        offense.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        offense.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        offense.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);

        // Defense - Physical Damage
        defend.armor.SetBaseValue(defaultStatSetup.armor);
        defend.evasion.SetBaseValue(defaultStatSetup.evasion);

        // Defense - Elemental Damage
        defend.fireResist.SetBaseValue(defaultStatSetup.fireResistance);
        defend.iceResist.SetBaseValue(defaultStatSetup.iceResistance);
        defend.lightningResist.SetBaseValue(defaultStatSetup.lightningResistance);
    }
}
