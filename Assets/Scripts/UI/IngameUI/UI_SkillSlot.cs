using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;
    private Image skillIcon;
    private RectTransform rect;
    private Button btn;

    private Skill_DataSO skillData;

    public SkillType skillType;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private string inputKeyName;
    [SerializeField] private TextMeshProUGUI intputKeyText;
    [SerializeField] private GameObject confictSlot;


    private void OnValidate()
    {
        gameObject.name = $"UI_SkillSlot - {skillType}";
    }


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        btn = GetComponent<Button>();
        skillIcon = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }


    public void SetupSkillSlot(Skill_DataSO selectedSkill)
    {
        this.skillData = selectedSkill;

        Color color = Color.black; color.a = 0.6f;
        cooldownImage.color = color;

        intputKeyText.text = inputKeyName;
        skillIcon.sprite = selectedSkill.icon;

        if (confictSlot != null)
        {
            confictSlot.SetActive(false);
        }
    }

    public void StartCooldown(float cooldownDuration)
    {
        cooldownImage.fillAmount = 1f;
        StartCoroutine(CooldownCo(cooldownDuration));
    }

    public void ResetCooldown()
    {
        cooldownImage.fillAmount = 0f;
        StopAllCoroutines();
    }

    private IEnumerator CooldownCo(float duration)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (timePassed / duration);
            yield return null;
        }

        cooldownImage.fillAmount = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillData == null)
        {
            return;
        }

        ui.skillToolTip.ShowToolTip(true, rect, skillData, null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, null);
    }
}
