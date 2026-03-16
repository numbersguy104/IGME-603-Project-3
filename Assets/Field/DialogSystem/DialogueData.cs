using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string[] speakerNames;
    [TextArea(2, 5)]
    public string[] lines;
}
