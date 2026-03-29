using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleStateManager : MonoBehaviour
{
    public static BattleStateManager Instance { get; private set; }

    [Header("Return Info")]
    public string returnSceneName;
    public Vector3 returnPlayerPosition;
    public bool returnAsSolid = true;

    [Header("Current Battle")]
    public string currentEnemyId;

    private readonly HashSet<string> defeatedEnemies = new HashSet<string>();
    private readonly HashSet<string> triggeredDialogues = new HashSet<string>();

    public float suppressBattleUntilTime;

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

    public void StartBattle(string enemyId, Vector3 playerPosition, bool activeIsSolid)
    {
        currentEnemyId = enemyId;
        returnPlayerPosition = playerPosition;
        returnSceneName = SceneManager.GetActiveScene().name;
        returnAsSolid = activeIsSolid;

        Debug.Log(
            $"[BattleStateManager] StartBattle | enemyId={enemyId} | returnScene={returnSceneName} | returnPos={returnPlayerPosition} | returnAsSolid={returnAsSolid}"
        );
    }

    public void MarkCurrentEnemyDefeated()
    {
        if (!string.IsNullOrEmpty(currentEnemyId))
            defeatedEnemies.Add(currentEnemyId);
    }

    public bool IsEnemyDefeated(string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId)) return false;
        return defeatedEnemies.Contains(enemyId);
    }

    public void MarkDialogueTriggered(string dialogueId)
    {
        if (string.IsNullOrEmpty(dialogueId)) return;
        triggeredDialogues.Add(dialogueId);
    }

    public bool IsDialogueTriggered(string dialogueId)
    {
        if (string.IsNullOrEmpty(dialogueId)) return false;
        return triggeredDialogues.Contains(dialogueId);
    }

    public void ClearCurrentBattle()
    {
        currentEnemyId = null;
    }
}