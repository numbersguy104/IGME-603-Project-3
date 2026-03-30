using UnityEngine;
using UnityEngine.Events;

public class InteractableDroppedItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemId;
    [SerializeField] private UnityEvent onInteracted;
    [SerializeField] private SkillData skillToLearn;
    [SerializeField] private bool isHugo;

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
        if(isHugo)
            CharacterStatsManager.Instance.hugo.LearnNewSkill(skillToLearn);
        else
            CharacterStatsManager.Instance.tenet.LearnNewSkill(skillToLearn);
        Destroy(gameObject);
    }
    
}
