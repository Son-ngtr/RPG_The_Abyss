using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat_MajorGroup major;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defend;

    public float GetPhysicalDamage(out bool isCriticalHit)
    {
        float baseDamage = offense.damage.GetValue();
        float bonusDamage = major.strength.GetValue() * 1f;
        float totalBaseDamage = baseDamage + bonusDamage;

        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * 0.3f; // Each point in agility gives 0.3% crit chance (+0.3% per AGI)
        float critChance = baseCritChance + bonusCritChance;

        float baseCritDamage = offense.critDamage.GetValue();
        float bonusCritDamage = major.strength.GetValue() * 0.5f; // Each point in strength gives 0.5% crit damage (+0.5% per STR)
        float critDamage = (baseCritDamage + bonusCritDamage) / 100;

        isCriticalHit = Random.Range(1, 100) < critChance;
        float finalDamage = isCriticalHit ? totalBaseDamage * critDamage : totalBaseDamage;

        return finalDamage;
    }

    public float GetArmorReduction()
    {
        float finalReduction = offense.armorReduction.GetValue() / 100;

        return finalReduction;
    }

    public float GetArmorMitigation(float armorReduction)
    {
        float armorValue = defend.armor.GetValue();
        float bonusArmor = major.vitality.GetValue() * 1f; // Each point in vitality gives 1 armor
        float totalArmor = armorValue + bonusArmor;

        float reductionMultiplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMultiplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100f); // Diminishing returns formula
        float mitigationCap = 0.85f; // Maximum mitigation percentage

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);
        return finalMitigation;
    }

    public float GetMaxHealth()
    {
        float baseMaxHealth = maxHealth.GetValue();
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
}
