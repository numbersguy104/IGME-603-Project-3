using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utility;

public class CombatManager : SingletonBehavior<CombatManager>
{
    public List<PlayerCharacter> playerCharacters;
    public List<Enemy> enemies;
    public List<PlayerCharacter_Combat> playerCharacters_Combat = new List<PlayerCharacter_Combat>();
    public List<Enemy_Combat> enemies_Combat = new List<Enemy_Combat>();
    
    public Team currentTurn;
    public int SkillPointRegenEveryTurn;
    public float expEarnedAfterCombat;
    
    public UnityEvent OnCombatStart;
    public UnityEvent OnPlayerTurnStart;
    public UnityEvent OnPlayerTurnEnd;
    public UnityEvent OnEnemyTurnStart;
    public UnityEvent OnEnemyTurnEnd;
    public UnityEvent OnCombatEnd;
    public UnityEvent OnCombatWin;
    public UnityEvent OnCombatLose;
    public void SwitchSide() {currentTurn = 1 - currentTurn;}

    
    void Start()
    {
        SampleCombat(); // For Test Only
    }

    void SampleCombat()
    {
        Enemy enemy = new Enemy(Resources.Load<EnemyData>("Test/Enemy"));

        //CharacterStatsManager should exist already in the main game
        //It may be null if entering combat scene directly from the editor
        if (CharacterStatsManager.Instance == null)
        {
            PlayerCharacter hugo = new PlayerCharacter(Resources.Load<PlayerData>("Test/Hugo"));
            PlayerCharacter tenet = new PlayerCharacter(Resources.Load<PlayerData>("Test/Tenet"));
            StartCombat(new List<PlayerCharacter> { hugo, tenet }, new List<Enemy> { enemy }, false);
        }
        else
        {
            StartCombat(CharacterStatsManager.Instance.characters, new List<Enemy> { enemy }, false);
        }
        currentTurn = Team.Player;
    }

    /// <summary>
    /// Combat Initialization.
    /// </summary>
    /// <param name="playerCharacters">List of the player characters involved in the combat</param>
    /// <param name="enemies"> List of the enemies involved in the combat</param>
    /// <param name="isFirstStrike"> True if this combat starts with the player's first strike</param>
    void StartCombat(List<PlayerCharacter> playerCharacters, List<Enemy> enemies, bool isFirstStrike)
    {
        GridManager.Instance.Init(9,9);
        
        this.playerCharacters = playerCharacters;
        this.enemies = enemies;
        
        CreateCombatCharacters();
        CreateEntities();
        
        PlayerController_Combat.Instance.Init(playerCharacters_Combat);
        CombatUI.Instance.Init(playerCharacters_Combat, enemies_Combat);
        
        OnCombatStart?.Invoke();
        StartCoroutine(Turns());
    }

    /// <summary>
    /// Create the combat instance for player and enemy characters
    /// </summary>
    public void CreateCombatCharacters()
    {
        // Create combat instance for each character
        foreach (var player in playerCharacters)
        {
            PlayerCharacter_Combat player_combat = new PlayerCharacter_Combat(player);
            playerCharacters_Combat.Add(player_combat);
            player_combat.OnCharacterDeath += OnNotifiedCharacterDeath;
        }

        foreach (var enemy in enemies)
        {
            Enemy_Combat enemy_combat = new Enemy_Combat(enemy);
            enemies_Combat.Add(enemy_combat);
            enemy_combat.OnCharacterDeath += OnNotifiedCharacterDeath;
        }
    }

    /// <summary>
    /// Create GameObjects (CombatEntities) for the characters to be instantiated on the board
    /// </summary>
    public void CreateEntities()
    {
        // Temp Combat Configuration
        for (int i = 0; i < playerCharacters.Count; i++)
        {
            PlayerCharacter character = playerCharacters[i];
            CombatEntity playerEntity = Instantiate(character.entityPrefab).GetComponent<CombatEntity>();
            playerCharacters_Combat[i].entity = playerEntity;
            playerEntity.character = playerCharacters_Combat[i];
            GridManager.Instance.Add(playerEntity.gameObject, i, 0, true);
        }
        
        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy character = enemies[i];
            CombatEntity enemyEntity = Instantiate(character.entityPrefab).GetComponent<CombatEntity>();
            enemyEntity.transform.rotation = Quaternion.Euler(0,180,0);
            enemies_Combat[i].entity = enemyEntity;
            enemyEntity.character = enemies_Combat[i];
            GridManager.Instance.Add(enemyEntity.gameObject, 8 - i, 8, true);
        }
        
        // TODO: Load Initial Character Positions on board From Asset?
    }

    /// <summary>
    /// The turn system
    /// </summary>
    IEnumerator Turns()
    {
        while (true)
        {
            CombatUI.Instance.SwitchTurn(true);
            
            OnPlayerTurnStart?.Invoke();
            CombatUI.Instance.UpdateCombatInfo();
            yield return new WaitWhile(() => currentTurn == Team.Player);
            OnPlayerTurnEnd?.Invoke();
            
            CombatUI.Instance.SwitchTurn(false);
            
            OnEnemyTurnStart?.Invoke();
            CombatUI.Instance.UpdateCombatInfo();
            CombatUI.Instance.SetPanelVisible(false);
            
            foreach (Enemy_Combat enemy in enemies_Combat)
            {
                yield return enemy.Act();
            } 
            EndTurn(Team.Enemy);
            OnEnemyTurnEnd?.Invoke();
            CombatUI.Instance.SetPanelVisible(true);
        }
    }

    /// <summary>
    /// Invoke this method to call for an end of one side's turn
    /// </summary>
    /// <param name="team">The team whose turn is to be ended</param>
    public void EndTurn(Team team)
    {
        if(currentTurn == team)
            SwitchSide();
    }

    /// <summary>
    /// Invoked when a character dies in the combat
    /// </summary>
    /// <param name="character">The character that dies</param>
    public void OnNotifiedCharacterDeath(Character_Combat character)
    {
        if (character is Enemy_Combat enemy)
        {
            character.OnCharacterDeath -= OnNotifiedCharacterDeath;
            enemies_Combat.Remove(enemy);
            
            if (enemies_Combat.Count == 0)
            {
                EndCombat(true);
            }
        }
        else
        {
            EndCombat(false);
            // TODO: What do we do when a player character Dies?
        }
    }

    /// <summary>
    /// Try to Flee the combat. Only works when it's player's turn
    /// </summary>
    public void TryFlee()
    {
        if (currentTurn == Team.Player) // TODO: Flee by chances
        {
            EndCombat(false);
        }
    }

    /// <summary>
    /// End the combat and reward the player.
    /// </summary>
    /// <param name="isWin">Set true if the player wins the combat</param>
    public void EndCombat(bool isWin)
    {
        OnCombatEnd?.Invoke();

        if (isWin)
        {
            OnCombatWin?.Invoke();
        }
        else
        {
            expEarnedAfterCombat = 0;
            OnCombatLose?.Invoke();
        }
        
        foreach (var character in playerCharacters)
        {
            character.GainExperience(expEarnedAfterCombat);
        }

        // TODO: Update the win/lose screen with amount of EXP earned

        foreach (var player_combat in playerCharacters_Combat)
            player_combat.OnCharacterDeath -= OnNotifiedCharacterDeath;
        foreach (var enemy_combat in enemies_Combat)
            enemy_combat.OnCharacterDeath -= OnNotifiedCharacterDeath;
        
        if (CharacterStatsManager.Instance != null) {
            CharacterStatsManager.Instance.hugo.UpdateStateFromCombat(playerCharacters_Combat[0]);
            CharacterStatsManager.Instance.tenet.UpdateStateFromCombat(playerCharacters_Combat[1]);
        }
    }

    public void ExitCombatScene()
    {
        SceneManager.LoadScene("SceneSettingNew");
    }
}