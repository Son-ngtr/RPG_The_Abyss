using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : MonoBehaviour
{
    [SerializeField] private Image speakerPortrait;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI dialogueChoices;

    [Space]
    [SerializeField] private float textSpeed = 0.1f;
    private string fullTextToShow;
    private Coroutine typingCoroutine;


    private void Awake()
    {
        ClearTexts();
    }


    public void PlayDialogueLine(DialogueLineSO line)
    {
        speakerPortrait.sprite = line.speaker.speakerPortrait;
        speakerName.text = line.speaker.speakerName;
        
        fullTextToShow = line.GetRandomLine();
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeTextCo(fullTextToShow));
    }

    public void DialogueInteraction()
    {
        if (typingCoroutine != null && dialogueText.text.Length > 5)
        {
            CompleteTyping();
            return;
        }
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = fullTextToShow;
        }
    }


    private IEnumerator TypeTextCo(string text)
    {
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text = dialogueText.text + letter;
            yield return new WaitForSeconds(textSpeed); 
        }
    }

    private void ClearTexts()
    {
        if (dialogueChoices != null) dialogueChoices.text = "";
        if (dialogueText != null) dialogueText.text = "";
        if (speakerName != null) speakerName.text = "";
        if (speakerPortrait != null) speakerPortrait.sprite = null;
    }
}
