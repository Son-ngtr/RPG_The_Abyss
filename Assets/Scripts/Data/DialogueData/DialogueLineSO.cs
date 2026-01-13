using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Dialogue Data/New line data", fileName = "Line - ")]

public class DialogueLineSO : ScriptableObject
{

    [Header("Dialogue Info")]
    public string dialogueGroupName;
    public DialogueSpeakerSO speaker;


    [Header("Text options")]
    [TextArea] public string[] textLine;

    [Header("Answer setup")]
    public bool playerCanAnswer; // If true, the player can choose a response after this line
    public DialogueLineSO[] answerLine;


    public string GetRandomLine()
    {
        if (textLine.Length == 0) return string.Empty;
        int randIndex = Random.Range(0, textLine.Length);
        return textLine[randIndex];
    }
}
