using System.Collections;
using UnityEngine;

public class EnemyInstance : MonoBehaviour
{
    [SerializeField] private string enemyId;
    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private Vector3 dropOffset = Vector3.zero;
    // [SerializeField] private SkillData skillToLearn;
    // [SerializeField] private bool isHugoCheck;

    [Header("Post Battle Dialogue")]
    [SerializeField] private SO_DialogueData defeatDialogue;
    [SerializeField] private string defeatDialogueId;

    public string EnemyId => enemyId;

    private void Start()
    {
        if (BattleStateManager.Instance == null) return;

        if (BattleStateManager.Instance.IsEnemyDefeated(enemyId))
        {
            StartCoroutine(HandleDefeatedRoutine());
        }
    }

    private IEnumerator HandleDefeatedRoutine() 
    { 
        yield return null;

        TryPlayDefeatDialogue();

        SpawnDrop();
        Destroy(gameObject);
    }

    private void TryPlayDefeatDialogue()
    {
        if (BattleStateManager.Instance == null) return;
        if (defeatDialogue == null) return;
        if (string.IsNullOrEmpty(defeatDialogueId)) return;
        if (DialogueManager.Instance == null) return;
        if (DialogueManager.Instance.IsPlaying) return;

        if (BattleStateManager.Instance.IsDialogueTriggered(defeatDialogueId))
            return;

        DialogueManager.Instance.StartDialogue(defeatDialogue);
        BattleStateManager.Instance.MarkDialogueTriggered(defeatDialogueId);

        Debug.Log($"[EnemyInstance] Played defeat dialogue: {defeatDialogueId}");
    }

    private void SpawnDrop()
    {
        if (droppedItemPrefab == null) return;

        GameObject drop = Instantiate(
            droppedItemPrefab,
            transform.position + dropOffset,
            Quaternion.identity
        );

        InteractableDroppedItem droppedItem = drop.GetComponent<InteractableDroppedItem>();
        if (droppedItem != null)
        {
            string generatedItemId = $"drop_{enemyId}";
            droppedItem.Initialize(generatedItemId);
        }
    }
}