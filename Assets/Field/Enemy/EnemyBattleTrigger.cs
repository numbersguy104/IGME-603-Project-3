using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class EnemyBattleTrigger : MonoBehaviour
{
    [SerializeField] private string combatSceneName = "CombatScene";
    [SerializeField] private EnemyInstance enemyInstance;

    private bool _triggered;

    private void Awake()
    {
        if (enemyInstance == null)
            enemyInstance = GetComponent<EnemyInstance>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;
        if (BattleStateManager.Instance == null) return;
        if (enemyInstance == null) return;

        _triggered = true;

        BattleStateManager.Instance.StartBattle(
            enemyInstance.EnemyId,
            other.transform.position
        );

        SceneManager.LoadScene(combatSceneName);
    }
}