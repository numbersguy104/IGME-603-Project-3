using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string speakerName;
    [TextArea(2, 5)]
    public string[] lines;
}