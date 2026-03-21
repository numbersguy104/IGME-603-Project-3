using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private CharacterTeamController teamController;

    private SO_DialogueData _currentDialogue;
    private int _currentIndex;
    private bool _isPlaying;

    public bool IsPlaying => _isPlaying;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!_isPlaying) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    public void StartDialogue(SO_DialogueData dialogue)
    {
        if (dialogue == null || dialogue.dialogueLines == null || dialogue.dialogueLines.Length == 0)
            return;

        _currentDialogue = dialogue;
        _currentIndex = 0;
        _isPlaying = true;

        if(teamController != null && teamController.Active != null)
        {
            teamController.Active.SetPlayerControlled(false);
        }

        ShowCurrentLine();
    }

    public void NextLine()
    {
        if (!_isPlaying || _currentDialogue == null)
            return;

        _currentIndex++;

        if (_currentIndex >= _currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    public void EndDialogue()
    {
        _isPlaying = false;
        _currentDialogue = null;
        _currentIndex = 0;

        if (teamController != null && teamController.Active != null)
            teamController.Active.SetPlayerControlled(true);

        if (dialogueUI != null)
            dialogueUI.Hide();
    }

    private void ShowCurrentLine()
    {
        if (dialogueUI == null || _currentDialogue == null)
            return;

        dialogueUI.Show(
            _currentDialogue.dialogueLines[_currentIndex].speakerName,
            _currentDialogue.dialogueLines[_currentIndex].text
        );
    }
}