using UnityEngine;
using UnityEngine.Events;

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

    public string ItemId => itemId;

    public void Initialize(string newItemId)
    {
        itemId = newItemId;
    }

    private void Start()
    {
        if (BattleStateManager.Instance == null) return;

        if (BattleStateManager.Instance.IsItemCollected(itemId))
        {
            Destroy(gameObject);
        }
    }

    public void Interact(GameObject interactor)
    {
        if (hasBeenInteracted) return;
        hasBeenInteracted = true;

        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.MarkItemCollected(itemId);
        }

        Debug.Log($"Item interacted by: {interactor.name}, itemId={itemId}");

        onInteracted?.Invoke();
        if (skillToLearn_Hugo != null)
        {
            CharacterStatsManager.Instance.hugo.LearnNewSkill(skillToLearn_Hugo);
            CharacterStatsManager.Instance.tenet.LearnNewSkill(skillToLearn_Tenet);
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
            // Healing potion
            CharacterStatsManager.Instance.hugo.Healed(20);
            CharacterStatsManager.Instance.tenet.Healed(20);
        }
        Destroy(gameObject);
    }
    
}
