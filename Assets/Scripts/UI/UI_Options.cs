using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Player player;

    [SerializeField] private Toggle healthBarToggle;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25f;

    [Header("BGM VOLUME SETTINGS")]
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private string bgmVolumeParameter;

    [Header("SFX VOLUME SETTINGS")]
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private string sfxVolumeParameter;

    [Header("HEALTH BAR TOGGLE SETTINGS")]
    private const string HEALTH_BAR_KEY = "ShowHealthBar";


    private void OnEnable()
    {  
        sfxVolumeSlider.value = PlayerPrefs.GetFloat(sfxVolumeParameter, 0.75f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat(bgmVolumeParameter, 0.75f);

        // Load trạng thái Health Bar Toggle
        bool showHealthBar = PlayerPrefs.GetInt(HEALTH_BAR_KEY, 1) == 1; // 1 = true, 0 = false
        healthBarToggle.isOn = showHealthBar;

    }

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        
        healthBarToggle.onValueChanged.AddListener(OnHealthBarToggleChange);
    }

    public void LoadupVolume()
    {
        sfxVolumeSlider.value = PlayerPrefs.GetFloat(sfxVolumeParameter, 0.75f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat(bgmVolumeParameter, 0.75f);
    }

    public void BGMSliderValue(float value)
    {
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(bgmVolumeParameter, newValue);
    }

    public void SFXSliderValue(float value)
    {
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(sfxVolumeParameter, newValue);
    }

    private void OnHealthBarToggleChange(bool isOn)
    {
        player?.health.EnableHealthBar(isOn);

        PlayerPrefs.SetInt(HEALTH_BAR_KEY, isOn ? 1 : 0);
    }

    public void GoMainMenuBtn() => GameManager.instance.ChangeScene("MainMenu", RespawnType.NonSpecific);


    private void OnDisable()
    {
        PlayerPrefs.SetFloat(sfxVolumeParameter, sfxVolumeSlider.value);
        PlayerPrefs.SetFloat(bgmVolumeParameter, bgmVolumeSlider.value);

        PlayerPrefs.SetInt(HEALTH_BAR_KEY, healthBarToggle.isOn ? 1 : 0);
    }
}
