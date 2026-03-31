using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// InteractableDroppedItem represents an item that can be interacted with and collected in the field.
/// Handles item collection, skill learning, healing, and pickup dialogue.
/// </summary>
public class InteractableDroppedItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemId;
    [SerializeField] private UnityEvent onInteracted;
    [SerializeField] private SkillData skillToLearn_Hugo;
    [SerializeField] private SkillData skillToLearn_Tenet;
    [SerializeField] private SO_DialogueData pickupDialogue;
    [SerializeField] private string pickupDialogueId;
    // [SerializeField] private bool isHugo;

    private bool hasBeenInteracted = false;

    // Public accessor for the item's unique ID.
    public string ItemId => itemId;

    /// <summary>
    /// Initializes the item with a new unique ID.
    /// </summary>
    /// <param name="newItemId">The new item ID to assign.</param>
    public void Initialize(string newItemId)
    {
        itemId = newItemId;
    }

    /// <summary>
    /// On start, checks if this item has already been collected.
    /// If so, destroys itself to prevent duplicate collection.
    /// </summary>
    private void Start()
    {
        if (BattleStateManager.Instance == null) return;

        if (BattleStateManager.Instance.IsItemCollected(itemId))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles interaction logic when the player interacts with the item.
    /// Marks the item as collected, triggers events, grants skills or healing, and plays pickup dialogue if applicable.
    /// </summary>
    /// <param name="interactor">The GameObject that interacted with this item.</param>
    public void Interact(GameObject interactor)
    {
        if (hasBeenInteracted) return;
        hasBeenInteracted = true;

        // Mark the item as collected in the battle state
        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.MarkItemCollected(itemId);
        }

        Debug.Log($"Item interacted by: {interactor.name}, itemId={itemId}");

        // Invoke any UnityEvents assigned in the inspector
        onInteracted?.Invoke();

        // If the item grants skills, teach them to both Hugo and Tenet
        if (skillToLearn_Hugo != null)
        {
            CharacterStatsManager.Instance.hugo.LearnNewSkill(skillToLearn_Hugo);
            CharacterStatsManager.Instance.tenet.LearnNewSkill(skillToLearn_Tenet);

            // Play pickup dialogue if available and not already playing
            if (BattleStateManager.Instance == null) return;
            if (DialogueManager.Instance == null) return;
            if (DialogueManager.Instance.IsPlaying) return;

            if (pickupDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(pickupDialogue);
                BattleStateManager.Instance.MarkDialogueTriggered(pickupDialogueId);
            }
        }
        else
        {
            // If not a skill item, heal both Hugo and Tenet
            CharacterStatsManager.Instance.hugo.Healed(40);
            CharacterStatsManager.Instance.tenet.Healed(40);
        }

        // Destroy the item after interaction
        Destroy(gameObject);
    }
}
