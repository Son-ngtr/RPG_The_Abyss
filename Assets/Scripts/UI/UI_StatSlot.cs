using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Entity_Stats playerStats;
    private RectTransform rectTransform;
    private UI ui;

    [SerializeField] private StatType statSlotType;
    [SerializeField] private TextMeshProUGUI statName;
    [SerializeField] private TextMeshProUGUI statValue;


    private void OnValidate()
    {
        gameObject.name = "UI_Stat - " + GetStatNameByType(statSlotType);
        statName.text = GetStatNameByType(statSlotType);
    }

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rectTransform = GetComponent<RectTransform>();
        playerStats = FindFirstObjectByType<Player_Stats>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.ShowToolTip(true, rectTransform, statSlotType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.ShowToolTip(false, null);
    }


    public void UpdateStatValue()
    {
        Stat statToUpdate = playerStats.GetStatByType(statSlotType);

        if (statToUpdate == null && statSlotType != StatType.ElementalDamage)
        {
            Debug.LogWarning("Stat of type " + statSlotType + " not found on player stats.");
            return;
        }

        float value = 0;
        switch (statSlotType)
        {
            // Major Stats
            case StatType.Strength:
                value = playerStats.major.strength.GetValue();
                break;
            case StatType.Agility:
                value = playerStats.major.agility.GetValue();
                break;
            case StatType.Intelligence:
                value = playerStats.major.intelligence.GetValue();
                break;
            case StatType.Vitality:
                value = playerStats.major.vitality.GetValue();
                break;

            // Offence stats
            case StatType.Damage:
                value = playerStats.GetBaseDamage(); // Damage includes bonus from strength // In future, we can specific value like (XX + YY) instead of total
                break;
            case StatType.CritChance:
                value = playerStats.GetCritChance();
                break;
            case StatType.CritPower:
                value = playerStats.GetCritDamage();
                break;
            case StatType.AttackSpeed:
                value = playerStats.offense.attackSpeed.GetValue() * 100; // Convert to percentage for display
                break;
            case StatType.ArmorReduction:
                value = playerStats.GetArmorReduction() * 100;
                break;

            // Defense stats
            case StatType.MaxHealth:
                value = playerStats.GetMaxHealth();
                break;
            case StatType.HealthRegen:
                value = playerStats.resources.healthRegen.GetValue();
                break;
            case StatType.Armor:
                value = playerStats.GetBaseArmor();
                break;
            case StatType.Evasion:
                value = playerStats.GetEvasion();
                break;

            // Elemental Damage Stats
            case StatType.FireDamage:
                value = playerStats.offense.fireDamage.GetValue();
                break;
            case StatType.IceDamage:
                value = playerStats.offense.iceDamage.GetValue();
                break;
            case StatType.LightningDamage:
                value = playerStats.offense.lightningDamage.GetValue();
                break;
            case StatType.ElementalDamage:
                value = playerStats.GetElementalDamage(out ElementType element, 1);
                break;


            // Elemental Resistance Stats
            case StatType.FireResistance:
                value = playerStats.GetElementalResistance(ElementType.Fire) * 100;
                break;
            case StatType.IceResistance:
                value = playerStats.GetElementalResistance(ElementType.Ice) * 100;
                break;
            case StatType.LightningResistance:
                value = playerStats.GetElementalResistance(ElementType.Lightning) * 100;
                break;
        }

        statValue.text = IsPercentageStat(statSlotType) ? value.ToString("F1") + "%" : value.ToString("F0");
    }

    private string GetStatNameByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return "Max Health";
            case StatType.HealthRegen: return "Health Regenaration";
            case StatType.Strength: return "Strength";
            case StatType.Agility: return "Agility";

            case StatType.Intelligence: return "Intelligence";
            case StatType.Vitality: return "Vitality";
            case StatType.AttackSpeed: return "Attack Speed";
            case StatType.Damage: return "Damage";
            case StatType.CritChance: return "Critical Chance";
            case StatType.CritPower: return "Critical Power";

            case StatType.ArmorReduction: return "Armor Reduction";
            case StatType.FireDamage: return "Fire Damage";
            case StatType.IceDamage: return "Ice Damage";
            case StatType.LightningDamage: return "Lightning Damage";
            case StatType.ElementalDamage: return "Elemental Damage";

            case StatType.Armor: return "Armor";
            case StatType.Evasion: return "Evasion";
            case StatType.IceResistance: return "Ice Resistance";
            case StatType.FireResistance: return "Fire Resistance";
            case StatType.LightningResistance: return "Lightning Resistance";

            default: return "Unknown Stat";
        }
    }

    private bool IsPercentageStat(StatType type)
    {
        switch (type)
        {
            case StatType.CritChance:
            case StatType.CritPower:
            case StatType.AttackSpeed:
            case StatType.ArmorReduction:
            case StatType.Evasion:
            case StatType.IceResistance:
            case StatType.FireResistance:
            case StatType.LightningResistance:
                return true;
            default:
                return false;
        }
    }

    
}
