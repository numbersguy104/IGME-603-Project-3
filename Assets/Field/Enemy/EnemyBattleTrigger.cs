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
        if (BattleStateManager.Instance != null &&
            Time.time < BattleStateManager.Instance.suppressBattleUntilTime)
        {
            return;
        }

        if (_triggered) return;
        if (!other.CompareTag("Player")) return;
        if (BattleStateManager.Instance == null) return;
        if (enemyInstance == null) return;

        CharacterTeamController team = FindFirstObjectByType<CharacterTeamController>();
        if (team == null || team.Active == null)
        {
            Debug.LogError("[EnemyBattleTrigger] CharacterTeamController or Active character not found.");
            return;
        }

        _triggered = true;

        BattleStateManager.Instance.StartBattle(
            enemyInstance.EnemyId,
            team.Active.transform.position,
            team.IsActiveSolid
        );

        SceneManager.LoadScene(combatSceneName);
    }
}