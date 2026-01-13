using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : MonoBehaviour
{
    private UI ui;
    private DialogueNpcData npcData;
    private Player_QuestManager playerQuestManager;

    [SerializeField] private Image speakerPortrait;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI[] dialogueChoicesText;

    #region TYPING SETTINGS
    public enum TypingMode
    {
        Constant,   // tốc độ cố định
        Natural,    // pause dài sau dấu câu + jitter ngẫu nhiên
        CustomCurve // tốc độ thay đổi theo AnimationCurve (x = 0..1 của progress)
    }

    [Space]
    [Header("Typing settings")]
    [SerializeField] private TypingMode typingMode = TypingMode.Natural;
    [Tooltip("Thời gian cơ bản giữa 2 ký tự (giây)")]
    [SerializeField] private float baseCharDelay = 0.035f;
    [Tooltip("Biến thiên ngẫu nhiên ± (giây) khi dùng Natural")]
    [SerializeField] private float jitter = 0.02f;
    [Tooltip("Hệ số nhân cho pause sau dấu câu (.,!,?)")]
    [SerializeField] private float punctuationPauseMultiplier = 12f;
    [Tooltip("Hệ số nhân cho pause sau dấu phẩy/semicolon")]
    [SerializeField] private float commaPauseMultiplier = 4f;

    [Tooltip("Nếu chọn CustomCurve: điều chỉnh delay = baseCharDelay * curve.Evaluate(progress)")]
    [SerializeField] private AnimationCurve customDelayCurve = AnimationCurve.Linear(0, 1, 1, 1);

    [Space]
    [SerializeField] private float minCharDelay = 0.005f; // giới hạn nhỏ nhất
    [SerializeField] private float maxCharDelay = 0.5f;   // giới hạn lớn nhất
    #endregion

    [Space]
    private string fullTextToShow;
    private Coroutine typingCoroutine;
    private DialogueLineSO currentLine;
    private DialogueLineSO[] currentChoices;
    private DialogueLineSO selectedChoice;
    private int selectedChoiceIndex;

    private bool waitingToConfirm;
    private bool canInteract;

    private List<DialogueLineSO> visibleChoices = new List<DialogueLineSO>();


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        playerQuestManager = Player.instance.questManager;
        ClearTexts();
    }

    public void SetupNpcData(DialogueNpcData npcData) => this.npcData = npcData;

    public void PlayDialogueLine(DialogueLineSO line) 
    {
        if (line == null) return;

        currentLine = line;
        currentChoices = line.choiceLines;
        canInteract = false;
        selectedChoice = null;
        selectedChoiceIndex = 0;

        HideAllChoices();

        if (speakerPortrait != null) speakerPortrait.sprite = line.speaker.speakerPortrait;
        if (speakerName != null) speakerName.text = line.speaker.speakerName;

        fullTextToShow = line.actionType == DialogueActionType.None || line.actionType == DialogueActionType.PlayerMakeChoice ? 
            line.GetRandomLine() :
            line.actionLine;

        typingCoroutine = StartCoroutine(TypeTextCo(fullTextToShow));
        StartCoroutine(EnableInteractionCo());
    }

    private void HandleNextAction()
    {
        switch (currentLine.actionType)
        {
            case DialogueActionType.OpenShop:
                ui.SwitchToInGameUI();
                ui.OpenMerchantUI(true);
                break;

            case DialogueActionType.PlayerMakeChoice:
                if (selectedChoice == null)
                {
                    ShowChoices();
                }
                else
                {
                    PlayDialogueLine(selectedChoice);
                }
                break;

            case DialogueActionType.OpenQuest:
                ui.SwitchToInGameUI();
                ui.OpenQuestUI(npcData.quests);
                break;

            case DialogueActionType.GetQuestReward:
                ui.SwitchToInGameUI();
                playerQuestManager.TryGetRewardFrom(npcData.rewardType);
                break;

            case DialogueActionType.OpenCraft:
                ui.SwitchToInGameUI();
                ui.OpenCraftUI(true);
                break;

            case DialogueActionType.CloseDialogue:
                ui.SwitchToInGameUI();
                break;
        }
    }

    public void DialogueInteraction()
    {
        if (canInteract == false)
        {
            return;
        }
        // nếu đang gõ — hoàn thành ngay
        if (typingCoroutine != null)
        {
            CompleteTyping();
            if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
            {
                waitingToConfirm = true;
            }
            else
            {
                HandleNextAction();
            }
            return;
        }

        // nếu đã hoàn thành, bạn có thể xử lý chuyển dòng tiếp theo ở đây (nếu cần)
        if (waitingToConfirm || selectedChoice != null)
        {
            waitingToConfirm = false;
            HandleNextAction();
        }
    }

    private void ShowChoices()
    {
        HideAllChoices();
        visibleChoices.Clear();

        // Lọc choice hợp lệ
        foreach (var choice in currentChoices)
        {
            if (choice.actionType == DialogueActionType.GetQuestReward &&
                playerQuestManager.HadCompletedQuest() == false)
            {
                continue;
            }

            visibleChoices.Add(choice);
        }

        selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, visibleChoices.Count - 1);

        for (int i = 0; i < dialogueChoicesText.Length; i++)
        {
            if (i >= visibleChoices.Count)
                break;

            var choice = visibleChoices[i];
            string choiceText = choice.playerChoiceAnswer;

            dialogueChoicesText[i].gameObject.SetActive(true);
            dialogueChoicesText[i].text =
                i == selectedChoiceIndex
                ? $"<color=yellow>> {choiceText}"
                : choiceText;
        }

        selectedChoice = visibleChoices.Count > 0 ? visibleChoices[selectedChoiceIndex] : null;
    }

    private void HideAllChoices()
    {
        foreach (var obj in dialogueChoicesText)
        {
            obj.gameObject.SetActive(false);
        }
    }

    public void NavigateChoice(int direction)
    {
        if (visibleChoices == null || visibleChoices.Count <= 1)
        {
            return;
        }

        selectedChoiceIndex += direction;
        selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, visibleChoices.Count - 1);
        ShowChoices();
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = fullTextToShow;
            typingCoroutine = null;
        }
    }


    private IEnumerator TypeTextCo(string text)
    {
        dialogueText.text = "";
        int length = string.IsNullOrEmpty(text) ? 0 : text.Length;

        for (int i = 0; i < length; i++)
        {
            char letter = text[i];
            dialogueText.text += letter;

            float delay = ComputeCharDelay(i, length, text, letter);
            yield return new WaitForSeconds(Mathf.Clamp(delay, minCharDelay, maxCharDelay));
        }

        // hoàn tất
        if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
        {
            waitingToConfirm = true;
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            selectedChoice = null;
            HandleNextAction();
        }
        typingCoroutine = null;
    }

    private IEnumerator EnableInteractionCo()
    {
        yield return null;
        canInteract = true;
    }

    private float ComputeCharDelay(int index, int totalLength, string text, char currentChar)
    {
        float delay = baseCharDelay;

        switch (typingMode)
        {
            case TypingMode.Constant:
                // giữ baseCharDelay
                break;

            case TypingMode.Natural:
                // jitter ngẫu nhiên
                delay = baseCharDelay + Random.Range(-jitter, jitter);

                // nếu ký tự hiện tại là dấu câu thì thêm pause
                if (IsSentenceEndPunctuation(currentChar))
                {
                    delay *= punctuationPauseMultiplier;
                }
                else if (IsCommaLike(currentChar))
                {
                    delay *= commaPauseMultiplier;
                }
                break;

            case TypingMode.CustomCurve:
                // progress từ 0..1 theo vị trí ký tự
                float progress = totalLength <= 1 ? 1f : (float)index / (totalLength - 1);
                float curveVal = customDelayCurve != null ? customDelayCurve.Evaluate(progress) : 1f;
                delay = baseCharDelay * curveVal;
                break;
        }

        // đảm bảo delay không âm
        return Mathf.Max(0f, delay);
    }

    private bool IsSentenceEndPunctuation(char c)
    {
        return c == '.' || c == '!' || c == '?';
    }

    private bool IsCommaLike(char c)
    {
        return c == ',' || c == ';' || c == ':';
    }

    private void ClearTexts()
    {
        //if (dialogueChoices != null) dialogueChoices. = "";
        if (dialogueText != null) dialogueText.text = "";
        if (speakerName != null) speakerName.text = "";
        if (speakerPortrait != null) speakerPortrait.sprite = null;
    }
}