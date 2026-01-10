using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private Player player;
    private UI_SkillSlot[] skillSlots;

    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    // player reference setup call when awake so other scripts can use it in their start methods
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        player.health.OnHealthUpdate += UpdateHealthBar;

        skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
    }

    public UI_SkillSlot GetSkillSlot(SkillType skillType)
    {
        if (skillSlots == null)
        {
            skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
        }

        foreach (var slot in skillSlots)
        {
            if (slot.skillType == skillType)
            {
                slot.gameObject.SetActive(true);
                return slot;
            }
        }

        return null;
    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.health.GetCurrentHealth());
        float maxHealth = Mathf.RoundToInt(player.stats.GetMaxHealth());
        float sizeDifference = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);

        if (sizeDifference > 0.1f)
        {
            healthRect.sizeDelta = new Vector2(maxHealth, healthRect.sizeDelta.y);
        }

        healthText.text = currentHealth + "/" + maxHealth;
        healthSlider.value = player.health.GetHealthPercent();
    }
}
