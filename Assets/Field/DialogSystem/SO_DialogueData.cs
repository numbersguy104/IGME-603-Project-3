using UnityEngine;
using System;

[Serializable]
public class DialogueLine
{
    public string speakerName;

    [TextArea(2, 5)]
    public string text;
}

[CreateAssetMenu(fileName = "SO_DialogueData", menuName = "Dialogue/SO Dialogue Data")]
public class SO_DialogueData : ScriptableObject
{
    public DialogueLine[] dialogueLines;
}

