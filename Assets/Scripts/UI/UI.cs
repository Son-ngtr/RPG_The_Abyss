using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject[] uiElements;
    public bool alternativeInput { get; private set; }
    private Player_InputSet inputSet;

    #region UI COMPONENTS
    public UI_SkillToolTip skillToolTip { get; private set; }
    public UI_ItemToolTip itemToolTip { get; private set; }
    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_StatToolTip statToolTip { get; private set; }
    public UI_Inventory inventoryUI { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_Craft craftUI { get; private set; }
    public UI_Merchant merchantUI { get; set; }
    public UI_InGame ingameUI { get; set; }
    public UI_Options optionUI { get; set; }
    public UI_DeathScreen deathScreenUI { get; set; }
    #endregion

    private bool skillTreeEnabled;
    private bool inventoryEnabled;

    private void Awake()
    {
        // TOOLTIPS
        skillToolTip = GetComponentInChildren<UI_SkillToolTip>();
        itemToolTip = GetComponentInChildren<UI_ItemToolTip>();
        statToolTip = GetComponentInChildren<UI_StatToolTip>();

        // MAIN UI PANELS
        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);
        inventoryUI = GetComponentInChildren<UI_Inventory>(true);
        storageUI = GetComponentInChildren<UI_Storage>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        merchantUI = GetComponentInChildren<UI_Merchant>(true);
        ingameUI = GetComponentInChildren<UI_InGame>(true);
        optionUI = GetComponentInChildren<UI_Options>(true);
        deathScreenUI = GetComponentInChildren<UI_DeathScreen>(true);

        skillTreeEnabled = skillTreeUI.gameObject.activeSelf; // Prevent double toggle on start
        inventoryEnabled = inventoryUI.gameObject.activeSelf;
    }

    private void Start()
    {
        skillTreeUI.UnlockDefaultSkills();
    }

    public void SetupControlUI(Player_InputSet inputSet)
    {
        this.inputSet = inputSet;

        inputSet.UI.ToggleSkillTreeUI.performed += ctx => ToggleSkillTreeUI();
        inputSet.UI.ToggleInventoryUI.performed += ctx => ToggleInventoryUI();

        inputSet.UI.AlternativeInput.performed += ctx => alternativeInput = true;
        inputSet.UI.AlternativeInput.canceled += ctx => alternativeInput = false;

        inputSet.UI.ToggleOptionUI.performed += ctx =>
        {
            foreach (var element in uiElements)
            {
                if (element.activeSelf)
                {
                    Time.timeScale = 1f; // Resume the game when closing options menu
                    SwitchToInGameUI();
                    return;
                }
            }

            Time.timeScale = 0f; // Pause the game when opening options menu
            OpenOptionsUI();
        };
    }

    public void OpenOptionsUI()
    {
        foreach (var element in uiElements)
        {
            element.gameObject.SetActive(false);
        }

        SwitchOffAllToolTips();
        StopPlayerControls(true);
        optionUI.gameObject.SetActive(true);
    }

    public void SwitchToInGameUI()
    {
        foreach (var element in uiElements)
        {
            element.gameObject.SetActive(false);
        }
        
        SwitchOffAllToolTips();
        StopPlayerControls(false);
        ingameUI.gameObject.SetActive(true);

        skillTreeEnabled = false;
        inventoryEnabled = false;
    }

    private void StopPlayerControls(bool stopControls)
    {
        if (stopControls)
        {
            inputSet.Player.Disable();
        }
        else
        {
            inputSet.Player.Enable();
        }
    }

    private void StopPlayerControlsIfNeeded()
    {
        foreach (var element in uiElements)
        {
            if (element.activeSelf)
            {
                StopPlayerControls(true);
                return;
            }
        }

        StopPlayerControls(false);
    }

    public void SwitchOffAllToolTips()
    {
        skillToolTip.ShowToolTip(false, null);
        itemToolTip.ShowToolTip(false, null);
        statToolTip.ShowToolTip(false, null);
    }

    public void SwitchOnAllToolTips()
    {
        skillToolTip.ShowToolTip(true, null);
        itemToolTip.ShowToolTip(true, null);
        statToolTip.ShowToolTip(true, null);
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeUI.transform.SetAsLastSibling(); // Bring skill tree UI to front
        SetToolTipsAboveOtherUIElements();

        skillTreeEnabled = !skillTreeEnabled;
        skillTreeUI.gameObject.SetActive(skillTreeEnabled);
        SwitchOffAllToolTips();

        StopPlayerControlsIfNeeded();
    }

    public void ToggleInventoryUI()
    {
        inventoryUI.transform.SetAsLastSibling(); // Bring inventory UI to front
        SetToolTipsAboveOtherUIElements();

        inventoryEnabled = !inventoryEnabled;
        inventoryUI.gameObject.SetActive(inventoryEnabled);
        SwitchOffAllToolTips();

        StopPlayerControlsIfNeeded();
    }

    public void OpenStorageUI(bool openStorageUI)
    {
        storageUI.gameObject.SetActive(openStorageUI);

        StopPlayerControls(openStorageUI);

        if (openStorageUI == false)
        {
            craftUI.gameObject.SetActive(false);
            SwitchOffAllToolTips();
        }
    }

    public void OpenMerchantUI(bool openMerchantUI)
    {
        merchantUI.gameObject.SetActive(openMerchantUI);

        StopPlayerControls(openMerchantUI);

        if (openMerchantUI == false)
        {
            SwitchOffAllToolTips();
        }
    }

    private void SetToolTipsAboveOtherUIElements()
    {
        itemToolTip.transform.SetAsLastSibling();
        skillToolTip.transform.SetAsLastSibling();
        statToolTip.transform.SetAsLastSibling();
    }

    public void OpenDeathScreenUI()
    {
        SwitchTo(deathScreenUI.gameObject);

        inputSet.Disable(); // pay attention when use gamepad in the future
    }

    private void SwitchTo(GameObject objectToSwitchOn)
    {
        foreach (var element in uiElements)
        {
            element.gameObject.SetActive(false);
        }

        objectToSwitchOn.SetActive(true);
    }
}
