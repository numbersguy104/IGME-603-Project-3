using UnityEngine;

public class EnemyInstance : MonoBehaviour
{
    [SerializeField] private string enemyId;

    public string EnemyId => enemyId;

    private void Start()
    {
        if (BattleStateManager.Instance == null) return;

        if (BattleStateManager.Instance.IsEnemyDefeated(enemyId))
        {
            Destroy(gameObject);
        }
    }
}