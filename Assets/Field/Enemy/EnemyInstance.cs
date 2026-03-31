using System.Collections;
using UnityEngine;

/// <summary>
/// EnemyInstance manages the state and post-battle behavior of an enemy in the field.
/// Handles defeat logic, post-battle dialogue, and item drops.
/// </summary>
public class EnemyInstance : MonoBehaviour
{
    [SerializeField] private string enemyId;
    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private Vector3 dropOffset = Vector3.zero;

    [Header("Post Battle Dialogue")]
    [SerializeField] private SO_DialogueData defeatDialogue;
    [SerializeField] private string defeatDialogueId;

    /// <summary>
    /// Public accessor for the enemy's unique ID.
    /// </summary>
    public string EnemyId => enemyId;

    private bool _resolvedAfterDefeat;

    /// <summary>
    /// On start, checks if this enemy has already been defeated.
    /// If so, initiates post-defeat handling (dialogue and drop).
    /// </summary>
    private void Start()
    {
        if (BattleStateManager.Instance == null)
            return;

        // Only proceed if this enemy has been defeated
        if (!BattleStateManager.Instance.IsEnemyDefeated(EnemyId))
            return;

        // Prevent duplicate handling
        if (_resolvedAfterDefeat)
            return;

        _resolvedAfterDefeat = true;
        StartCoroutine(HandleDefeatedRoutine());
    }

    /// <summary>
    /// Coroutine that waits for field restoration to complete, then plays defeat dialogue and spawns drop.
    /// </summary>
    private IEnumerator HandleDefeatedRoutine()
    {
        // Wait until restoration is complete or BattleStateManager is unavailable
        yield return new WaitUntil(() =>
            BattleStateManager.Instance == null ||
            BattleStateManager.Instance.restoreCompleted
        );

        yield return null;

        TryPlayDefeatDialogue();
        SpawnDrop();

        // Remove enemy from the scene after handling
        Destroy(gameObject);
    }

    /// <summary>
    /// Attempts to play the defeat dialogue if not already triggered and all conditions are met.
    /// </summary>
    private void TryPlayDefeatDialogue()
    {
        if (BattleStateManager.Instance == null) return;
        if (defeatDialogue == null) return;
        if (string.IsNullOrEmpty(defeatDialogueId)) return;
        if (DialogueManager.Instance == null) return;
        if (DialogueManager.Instance.IsPlaying) return;

        // Prevent replaying the same dialogue
        if (BattleStateManager.Instance.IsDialogueTriggered(defeatDialogueId))
            return;

        DialogueManager.Instance.StartDialogue(defeatDialogue);
        BattleStateManager.Instance.MarkDialogueTriggered(defeatDialogueId);

        Debug.Log($"[EnemyInstance] Played defeat dialogue: {defeatDialogueId}");
    }

    /// <summary>
    /// Spawns the dropped item prefab at the enemy's position if not already collected.
    /// </summary>
    private void SpawnDrop()
    {
        if (droppedItemPrefab == null) return;

        string generatedItemId = $"drop_{enemyId}";
        // Prevent duplicate drops if already collected
        if (BattleStateManager.Instance != null &&
            BattleStateManager.Instance.IsItemCollected(generatedItemId))
        {
            return;
        }

        GameObject drop = Instantiate(
            droppedItemPrefab,
            transform.position + dropOffset,
            Quaternion.identity
        );

        // Initialize the dropped item with its unique ID
        InteractableDroppedItem droppedItem = drop.GetComponent<InteractableDroppedItem>();
        if (droppedItem != null)
        {
            droppedItem.Initialize(generatedItemId);
        }
    }
}
