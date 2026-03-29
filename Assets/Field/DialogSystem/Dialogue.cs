using UnityEngine;

public class Dialogue : MonoBehaviour, IInteractable
{
    public enum TriggerMode
    {
        InteractKey,
        AutoOnTrigger,
        AutoOnCollision
    }

    [Header("Dialogue")]
    [SerializeField] private SO_DialogueData dialogueData;

    [Header("Persistent ID")]
    [SerializeField] private string dialogueId;

    [Header("Trigger")]
    [SerializeField] private TriggerMode triggerMode = TriggerMode.InteractKey;
    [SerializeField] private bool canRepeat = true;
    [SerializeField] private bool onlyPlayerCanTrigger = true;

    private bool _hasTriggered;

    public TriggerMode Mode => triggerMode;

    private void Awake()
    {
        if (!canRepeat && BattleStateManager.Instance != null)
        {
            _hasTriggered = BattleStateManager.Instance.IsDialogueTriggered(dialogueId);
        }
    }

    public void Interact(GameObject interactor)
    {
        if (triggerMode != TriggerMode.InteractKey)
            return;

        TryStartDialogue(interactor);
    }

    public void NotifyTriggerEnter(GameObject other)
    {
        if (triggerMode != TriggerMode.AutoOnTrigger)
            return;

        TryStartDialogue(other);
    }

    public void NotifyCollisionEnter(GameObject other)
    {
        if (triggerMode != TriggerMode.AutoOnCollision)
            return;

        TryStartDialogue(other);
    }

    private void TryStartDialogue(GameObject other)
    {
        if (!canRepeat && _hasTriggered)
            return;

        if (DialogueManager.Instance == null || DialogueManager.Instance.IsPlaying)
            return;

        if (onlyPlayerCanTrigger && !other.CompareTag("Player"))
            return;

        DialogueManager.Instance.StartDialogue(dialogueData);

        if (!canRepeat)
        {
            _hasTriggered = true;

            if (BattleStateManager.Instance != null)
            {
                BattleStateManager.Instance.MarkDialogueTriggered(dialogueId);
            }
        }
    }
}