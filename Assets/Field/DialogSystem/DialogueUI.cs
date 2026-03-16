using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject dialogueRoot;

    [Header("Text")]
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueLineText;

    private void Awake()
    {
        Hide();
    }

    public void Show(string speakerName, string line)
    {
        if (dialogueRoot != null)
            dialogueRoot.SetActive(true);

        if (speakerNameText != null)
            speakerNameText.text = speakerName;

        if (dialogueLineText != null)
            dialogueLineText.text = line;

    }

    public void Hide()
    {
        if (dialogueRoot != null)
            dialogueRoot.SetActive(false);
    }
}