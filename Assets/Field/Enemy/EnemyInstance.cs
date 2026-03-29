using UnityEngine;

public class EnemyInstance : MonoBehaviour
{
    [SerializeField] private string enemyId;
    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private Vector3 dropOffset = Vector3.zero;

    public string EnemyId => enemyId;

    private void Start()
    {
        if (BattleStateManager.Instance == null) return;

        if (BattleStateManager.Instance.IsEnemyDefeated(enemyId))
        {
            HandleDefeated();
        }
    }

    public void HandleDefeated()
    {
        SpawnDrop();
        Destroy(gameObject);
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