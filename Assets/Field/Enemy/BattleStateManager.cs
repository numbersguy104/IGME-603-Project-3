using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// BattleStateManager manages the state transitions between field and battle scenes,
/// tracks defeated enemies, triggered dialogues, collected items, and handles restoration after combat.
/// Implements a singleton pattern to persist across scenes.
/// </summary>
public class BattleStateManager : MonoBehaviour
{

    // Singleton instance of the BattleStateManager.
    public static BattleStateManager Instance { get; private set; }

    // The name of the scene to return to after combat.
    [Header("Return Info")]
    public string returnSceneName;

    // The position to return the player to after combat.
    public Vector3 returnPlayerPosition;

    // Whether the player should be returned as a solid object (not ghosted).
    public bool returnAsSolid = true;

    // The ID of the current enemy being battled.
    [Header("Current Battle")]
    public string currentEnemyId;

    // Indicates if the game is currently returning from combat.
    [Header("Restore State")]
    public bool isReturningFromCombat;

    // Indicates if the field state restoration after combat is completed.
    public bool restoreCompleted;

    // Sets to track defeated enemies, triggered dialogues, and collected items
    private readonly HashSet<string> defeatedEnemies = new HashSet<string>();
    private readonly HashSet<string> triggeredDialogues = new HashSet<string>();
    private readonly HashSet<string> collectedItems = new HashSet<string>();

    // Time until which battles are suppressed after returning from combat.
    public float suppressBattleUntilTime;

    // Ensures only one instance exists and persists across scene loads.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Initializes battle state and stores return information before entering combat.
    /// </summary>
    /// <param name="enemyId">The ID of the enemy being battled.</param>
    /// <param name="playerPosition">The player's position to return to after combat.</param>
    /// <param name="activeIsSolid">Whether the player should be solid after returning.</param>
    public void StartBattle(string enemyId, Vector3 playerPosition, bool activeIsSolid)
    {
        currentEnemyId = enemyId;
        returnPlayerPosition = playerPosition;
        returnSceneName = SceneManager.GetActiveScene().name;
        returnAsSolid = activeIsSolid;

        isReturningFromCombat = false;
        restoreCompleted = false;

        Debug.Log(
            $"[BattleStateManager] StartBattle | enemyId={enemyId} | returnScene={returnSceneName} | returnPos={returnPlayerPosition} | returnAsSolid={returnAsSolid}"
        );
    }

    /// <summary>
    /// Prepares the state for returning from combat, suppressing battles for a short duration.
    /// </summary>
    /// <param name="suppressDuration">Duration to suppress battles after returning.</param>
    public void PrepareReturnFromCombat(float suppressDuration = 1.5f)
    {
        isReturningFromCombat = true;
        restoreCompleted = false;
        suppressBattleUntilTime = Time.time + suppressDuration;

        Debug.Log(
            $"[BattleStateManager] PrepareReturnFromCombat | suppressUntil={suppressBattleUntilTime}"
        );
    }

    /// <summary>
    /// Marks the restoration process as completed and resets return state.
    /// </summary>
    public void MarkRestoreCompleted()
    {
        restoreCompleted = true;
        isReturningFromCombat = false;

        Debug.Log("[BattleStateManager] Restore completed.");
    }

    /// <summary>
    /// Marks the current enemy as defeated by adding its ID to the defeated set.
    /// </summary>
    public void MarkCurrentEnemyDefeated()
    {
        if (!string.IsNullOrEmpty(currentEnemyId))
            defeatedEnemies.Add(currentEnemyId);
    }

    /// <summary>
    /// Checks if a specific enemy has been defeated.
    /// </summary>
    /// <param name="enemyId">The enemy ID to check.</param>
    /// <returns>True if the enemy has been defeated, false otherwise.</returns>
    public bool IsEnemyDefeated(string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId)) return false;
        return defeatedEnemies.Contains(enemyId);
    }

    /// <summary>
    /// Marks a dialogue as triggered by adding its ID to the triggered set.
    /// </summary>
    /// <param name="dialogueId">The dialogue ID to mark as triggered.</param>
    public void MarkDialogueTriggered(string dialogueId)
    {
        if (string.IsNullOrEmpty(dialogueId)) return;
        triggeredDialogues.Add(dialogueId);
    }

    /// <summary>
    /// Checks if a specific dialogue has been triggered.
    /// </summary>
    /// <param name="dialogueId">The dialogue ID to check.</param>
    /// <returns>True if the dialogue has been triggered, false otherwise.</returns>
    public bool IsDialogueTriggered(string dialogueId)
    {
        if (string.IsNullOrEmpty(dialogueId)) return false;
        return triggeredDialogues.Contains(dialogueId);
    }

    /// <summary>
    /// Marks an item as collected by adding its ID to the collected set.
    /// </summary>
    /// <param name="itemId">The item ID to mark as collected.</param>
    public void MarkItemCollected(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return;
        collectedItems.Add(itemId);
    }

    /// <summary>
    /// Checks if a specific item has been collected.
    /// </summary>
    /// <param name="itemId">The item ID to check.</param>
    /// <returns>True if the item has been collected, false otherwise.</returns>
    public bool IsItemCollected(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return false;
        return collectedItems.Contains(itemId);
    }

    /// <summary>
    /// Clears the current battle state by resetting the current enemy ID.
    /// </summary>
    public void ClearCurrentBattle()
    {
        currentEnemyId = null;
    }
}
