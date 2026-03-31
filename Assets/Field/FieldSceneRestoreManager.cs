using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FieldSceneRestoreCoordinator : MonoBehaviour
{
    [SerializeField] private float reEnableTriggerDelay = 0.3f;

    private IEnumerator Start()
    {
        Debug.Log("[FieldSceneRestoreCoordinator] Start called.");

        yield return null;

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

        yield return RestoreFieldAfterCombat();
    }

    private IEnumerator RestoreFieldAfterCombat()
    {
        Debug.Log("[FieldSceneRestoreCoordinator] Restore start.");

        yield return new WaitForFixedUpdate();

        CharacterTeamController team = FindFirstObjectByType<CharacterTeamController>();
        CameraRigFollow3D camRig = FindFirstObjectByType<CameraRigFollow3D>();
        EnemyBattleTrigger[] triggers = FindObjectsByType<EnemyBattleTrigger>(FindObjectsSortMode.None);

        if (team == null)
        {
            Debug.LogError("[FieldSceneRestoreCoordinator] CharacterTeamController not found.");
            yield break;
        }

        if (camRig != null)
            camRig.SetFollowEnabled(false);

        foreach (var trigger in triggers)
            trigger.enabled = false;

        team.SetInputEnabled(false);

        Debug.Log(
            $"[FieldSceneRestoreCoordinator] Applying restored state | pos={BattleStateManager.Instance.returnPlayerPosition} | " +
            $"useSolid={BattleStateManager.Instance.returnAsSolid}"
        );

        team.ApplyRestoredFieldState(
            true,
            BattleStateManager.Instance.returnPlayerPosition
        );

        Physics.SyncTransforms();

        if (camRig != null)
            camRig.SnapToTargetNow();

        yield return null;

        if (camRig != null)
            camRig.SnapToTargetNow();

        team.SetInputEnabled(true);

        if (camRig != null)
            camRig.SetFollowEnabled(true);

        yield return new WaitForSeconds(reEnableTriggerDelay);

        foreach (var trigger in triggers)
            trigger.enabled = true;

        BattleStateManager.Instance.MarkRestoreCompleted();
        BattleStateManager.Instance.ClearCurrentBattle();

        Debug.Log("[FieldSceneRestoreCoordinator] Restore finished.");
    }
}