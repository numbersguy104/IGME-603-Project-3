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

        if (speakerNameText != null)
        {
            speakerNameText.text = speakerName;
            Debug.Log(speakerName);
            switch (speakerName)
            {
                case "Hugo":
                    if (HugoIcon != null)
                        speakerIcon.color = Color.white;
                    speakerIcon.sprite = HugoIcon;
                    break;
                case "Tenet":
                    if (TenetIcon != null)
                        speakerIcon.color = Color.white;
                    speakerIcon.sprite = TenetIcon;
                    break;
                case "Resident":
                    if (ResidentIcon != null)
                        speakerIcon.color = Color.white;
                    speakerIcon.sprite = ResidentIcon;
                    break;
                default:
                    speakerIcon.sprite = null;
                    break;
            }

            if (speakerName == "Tenet")
            {
                Debug.Log("True");
                speakerIcon.sprite = TenetIcon;
            }

            if (dialogueLineText != null)
                dialogueLineText.text = line;
        }

    }

    public void Hide()
    {
        if (dialogueRoot != null)
            dialogueRoot.SetActive(false);
    }
}