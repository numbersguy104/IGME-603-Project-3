using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject dialogueRoot;

    [Header("Text")]
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueLineText;

    [Header("Character Icon")]
    [SerializeField] private Sprite HugoIcon;
    [SerializeField] private Sprite TenetIcon;
    [SerializeField] private Sprite ResidentIcon;
    [SerializeField] private Image speakerIcon;

    private void Awake()
    {
        Hide();
    }

    public void Show(string speakerName, string line)
    {
        if (dialogueRoot != null)
            dialogueRoot.SetActive(true);

        string cleanName = speakerName.Trim();
        Debug.Log($"speakerName = [{cleanName}]");

        if (speakerNameText != null)
            speakerNameText.text = cleanName;

        if (dialogueLineText != null)
            dialogueLineText.text = line;

        if (speakerIcon != null)
        {

            switch (cleanName)
            {
                case "HUGO":
                    speakerIcon.sprite = HugoIcon;
                    speakerIcon.color = new Color(1f, 1f, 1f, 1f);
                    break;

                case "TENET":
                    speakerIcon.sprite = TenetIcon;
                    speakerIcon.color = new Color(1f, 1f, 1f, 1f);
                    break;

                case "SOLEMN RESIDENT":
                    speakerIcon.sprite = ResidentIcon;
                    speakerIcon.color = new Color(1f, 1f, 1f, 1f);
                    break;

                default:
                    Debug.LogWarning($"No matching icon for speaker: [{cleanName}]");
                    speakerIcon.sprite = null;
                    speakerIcon.color = new Color(1f, 1f, 1f, 0f);
                    break;
            }
        }
    }

    public void Hide()
    {
        if (dialogueRoot != null)
            dialogueRoot.SetActive(false);
    }
}