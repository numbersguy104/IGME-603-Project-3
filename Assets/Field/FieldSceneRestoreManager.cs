using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// FieldSceneRestoreCoordinator handles restoring the field scene state after returning from combat.
/// It coordinates player position, camera, triggers, and input restoration.
/// </summary>
public class FieldSceneRestoreManager : MonoBehaviour
{
    [SerializeField] private float reEnableTriggerDelay = 0.3f;

    /// <summary>
    /// Entry point coroutine that checks if restoration is needed and initiates the restore process.
    /// </summary>
    private IEnumerator Start()
    {
        Debug.Log("[FieldSceneRestoreCoordinator] Start called.");

        // Wait one frame to ensure all objects are initialized
        yield return null;

        // Ensure BattleStateManager is available
        if (BattleStateManager.Instance == null)
        {
            Debug.LogError("[FieldSceneRestoreCoordinator] BattleStateManager.Instance is NULL");
            yield break;
        }

        Debug.Log(
            $"[FieldSceneRestoreCoordinator] scene={SceneManager.GetActiveScene().name} | " +
            $"returnScene={BattleStateManager.Instance.returnSceneName} | " +
            $"isReturningFromCombat={BattleStateManager.Instance.isReturningFromCombat} | " +
            $"restoreCompleted={BattleStateManager.Instance.restoreCompleted} | " +
            $"savedPos={BattleStateManager.Instance.returnPlayerPosition} | " +
            $"returnAsSolid={BattleStateManager.Instance.returnAsSolid}"
        );

        // Only restore if returning from combat and in the correct scene
        if (!BattleStateManager.Instance.isReturningFromCombat)
        {
            Debug.Log("[FieldSceneRestoreCoordinator] Not returning from combat, skip restore.");
            yield break;
        }

        if (SceneManager.GetActiveScene().name != BattleStateManager.Instance.returnSceneName)
        {
            Debug.Log("[FieldSceneRestoreCoordinator] Current scene is not return scene, skip restore.");
            yield break;
        }

        // Begin restoration process
        yield return RestoreFieldAfterCombat();
    }

    /// <summary>
    /// Coroutine that restores the field state after combat, including player position, camera, triggers, and input.
    /// </summary>
    private IEnumerator RestoreFieldAfterCombat()
    {
        Debug.Log("[FieldSceneRestoreCoordinator] Restore start.");

        // Wait for physics update to ensure transforms are up to date
        yield return new WaitForFixedUpdate();

        // Find required components in the scene
        CharacterTeamController team = FindFirstObjectByType<CharacterTeamController>();
        CameraRigFollow3D camRig = FindFirstObjectByType<CameraRigFollow3D>();
        EnemyBattleTrigger[] triggers = FindObjectsByType<EnemyBattleTrigger>(FindObjectsSortMode.None);

        // Ensure player team controller exists
        if (team == null)
        {
            Debug.LogError("[FieldSceneRestoreCoordinator] CharacterTeamController not found.");
            yield break;
        }

        // Temporarily disable camera follow and enemy triggers
        if (camRig != null)
            camRig.SetFollowEnabled(false);

        foreach (var trigger in triggers)
            trigger.enabled = false;

        // Disable player input during restoration
        team.SetInputEnabled(false);

        Debug.Log(
            $"[FieldSceneRestoreCoordinator] Applying restored state | pos={BattleStateManager.Instance.returnPlayerPosition} | " +
            $"useSolid={BattleStateManager.Instance.returnAsSolid}"
        );

        // Restore player position and state
        team.ApplyRestoredFieldState(
            true,
            BattleStateManager.Instance.returnPlayerPosition
        );

        // Sync transforms to ensure camera and player are updated
        Physics.SyncTransforms();

        // Snap camera to the restored player position
        if (camRig != null)
            camRig.SnapToTargetNow();

        yield return null;

        // Snap camera again after one frame to ensure correct alignment
        if (camRig != null)
            camRig.SnapToTargetNow();

        // Re-enable player input
        team.SetInputEnabled(true);

        // Re-enable camera follow
        if (camRig != null)
            camRig.SetFollowEnabled(true);

        // Wait before re-enabling enemy triggers to avoid immediate retriggering
        yield return new WaitForSeconds(reEnableTriggerDelay);

        foreach (var trigger in triggers)
            trigger.enabled = true;

        // Mark restoration as complete and clear battle state
        BattleStateManager.Instance.MarkRestoreCompleted();
        BattleStateManager.Instance.ClearCurrentBattle();

        Debug.Log("[FieldSceneRestoreCoordinator] Restore finished.");
    }
}