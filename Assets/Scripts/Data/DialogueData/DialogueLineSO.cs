using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Dialogue Data/New line data", fileName = "Line - ")]

public class DialogueLineSO : ScriptableObject
{

    [Header("Dialogue Info")]
    public string dialogueGroupName;
    public DialogueSpeakerSO speaker;

    [Header("Text options")]
    [TextArea] public string[] textLine; // What npc says

    [Header("Choice Info")]
    [TextArea] public string playerChoiceAnswer; // What player says when making this choice
    public DialogueLineSO[] choiceLines;

    [Header("Dialogue Action")]
    [TextArea] public string actionLine; // What npc says when performing action
    public DialogueActionType actionType;


    public string GetFirstLine()
    {
        if (textLine.Length == 0) return string.Empty;
        return textLine[0];
    }

    public string GetRandomLine()
    {
        if (textLine.Length == 0) return string.Empty;
        int randIndex = Random.Range(0, textLine.Length);
        return textLine[randIndex];
    }
}
