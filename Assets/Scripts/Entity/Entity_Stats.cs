using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat_MajorGroup major;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defend;

    public float GetMaxHealth()
    {
        float baseHealth = maxHealth.GetValue();
        float bonusHealth = major.vitality.GetValue() * 5;

        return baseHealth + bonusHealth;
    }

    public float GetEvasion()
    {
        float baseAvasion = defend.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 0.5f; // Each point in agility gives 0.5% evasion

        float totalEvation = baseAvasion + bonusEvasion;
        float evasionCap = 75f; // Maximum evasion percentage

        float finalEvasion = Mathf.Clamp(totalEvation, 0, evasionCap);

        return finalEvasion;
    }
}
