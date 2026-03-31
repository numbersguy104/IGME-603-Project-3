using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// EnemyBattleTrigger detects when the player enters its collider and initiates a battle if conditions are met.
/// It checks for enemy defeat state, battle suppression, and manages transition to the combat scene.
/// </summary>
[RequireComponent(typeof(Collider))]
public class EnemyBattleTrigger : MonoBehaviour
{
    [SerializeField] private string combatSceneName = "CombatScene";
    [SerializeField] private EnemyInstance enemyInstance;

    private bool _triggered;

    /// <summary>
    /// Ensures the enemyInstance reference is set, attempting to get it from the same GameObject if not assigned.
    /// </summary>
    private void Awake()
    {
        if (enemyInstance == null)
            enemyInstance = GetComponent<EnemyInstance>();
    }

    /// <summary>
    /// Called when another collider enters this trigger.
    /// Checks all conditions before starting a battle and loading the combat scene.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Prevent multiple triggers
        if (_triggered) return;

        // Only trigger on player
        if (!other.CompareTag("Player")) return;

        // Ensure BattleStateManager and enemyInstance are available
        if (BattleStateManager.Instance == null) return;
        if (enemyInstance == null) return;

        // Do not trigger if returning from combat
        if (BattleStateManager.Instance.isReturningFromCombat) return;

        // Prevent triggering if restoration is not completed and suppression is active
        if (!BattleStateManager.Instance.restoreCompleted &&
            BattleStateManager.Instance.suppressBattleUntilTime > 0f)
            return;

        // Prevent triggering if within suppression time window
        if (Time.time < BattleStateManager.Instance.suppressBattleUntilTime)
            return;

        // Do not trigger if this enemy has already been defeated
        if (BattleStateManager.Instance.IsEnemyDefeated(enemyInstance.EnemyId))
            return;

        // Find the player's team controller and active character
        CharacterTeamController team = FindFirstObjectByType<CharacterTeamController>();
        if (team == null || team.Active == null)
        {
            Debug.LogError("[EnemyBattleTrigger] CharacterTeamController or Active character not found.");
            return;
        }

        // Mark as triggered to prevent re-entry
        _triggered = true;

        // Store battle state and player position before switching scenes
        BattleStateManager.Instance.StartBattle(
            enemyInstance.EnemyId,
            team.Active.transform.position,
            team.IsActiveSolid
        );

        // Load the combat scene
        SceneManager.LoadScene(combatSceneName);
    }
}
