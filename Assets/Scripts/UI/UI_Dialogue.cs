using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : MonoBehaviour
{
    public enum TypingMode
    {
        Constant,   // tốc độ cố định
        Natural,    // pause dài sau dấu câu + jitter ngẫu nhiên
        CustomCurve // tốc độ thay đổi theo AnimationCurve (x = 0..1 của progress)
    }

    [SerializeField] private Image speakerPortrait;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI dialogueChoices;

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

    private string fullTextToShow;
    private Coroutine typingCoroutine;


    private void Awake()
    {
        ClearTexts();
    }


    public void PlayDialogueLine(DialogueLineSO line)
    {
        if (line == null) return;

        if (speakerPortrait != null) speakerPortrait.sprite = line.speaker.speakerPortrait;
        if (speakerName != null) speakerName.text = line.speaker.speakerName;

        fullTextToShow = line.GetRandomLine();

        // dừng coroutine cũ nếu có
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        typingCoroutine = StartCoroutine(TypeTextCo(fullTextToShow));
    }

    public void DialogueInteraction()
    {
        // nếu đang gõ — hoàn thành ngay
        if (typingCoroutine != null && dialogueText != null && dialogueText.text.Length > 6 && dialogueText.text != fullTextToShow)
        {
            CompleteTyping();
            return;
        }

        // nếu đã hoàn thành, bạn có thể xử lý chuyển dòng tiếp theo ở đây (nếu cần)
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        if (dialogueText != null) dialogueText.text = fullTextToShow;
    }


    private IEnumerator TypeTextCo(string text)
    {
        if (dialogueText == null)
        {
            typingCoroutine = null;
            yield break;
        }

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
        typingCoroutine = null;
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
        if (dialogueChoices != null) dialogueChoices.text = "";
        if (dialogueText != null) dialogueText.text = "";
        if (speakerName != null) speakerName.text = "";
        if (speakerPortrait != null) speakerPortrait.sprite = null;
    }
}