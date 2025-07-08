using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Container")]

public class DialogueText : ScriptableObject
{
    public string speakerName;
    [TextArea(3,10)]
    public string[] paragraphs;
}
