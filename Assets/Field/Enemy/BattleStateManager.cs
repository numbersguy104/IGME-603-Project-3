using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleStateManager : MonoBehaviour
{
    public static BattleStateManager Instance { get; private set; }

    [Header("Return Info")]
    public string returnSceneName;
    public Vector3 returnPlayerPosition;

    [Header("Current Battle")]
    public string currentEnemyId;

    private readonly HashSet<string> defeatedEnemies = new HashSet<string>();

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

    public void StartBattle(string enemyId, Vector3 playerPosition)
    {
        currentEnemyId = enemyId;
        returnPlayerPosition = playerPosition;
        returnSceneName = SceneManager.GetActiveScene().name;
    }

    public void MarkCurrentEnemyDefeated()
    {
        if (!string.IsNullOrEmpty(currentEnemyId))
        {
            defeatedEnemies.Add(currentEnemyId);
        }
    }

    public bool IsEnemyDefeated(string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId)) return false;
        return defeatedEnemies.Contains(enemyId);
    }

    public void ClearCurrentBattle()
    {
        currentEnemyId = null;
    }
}
