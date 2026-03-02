using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utility;

public class CombatManager : SingletonBehavior<CombatManager>
{
    public List<PlayerCharacter> playerCharacters;
    public List<Enemy> enemies;
    public List<PlayerCharacter_Combat> playerCharacters_Combat = new List<PlayerCharacter_Combat>();
    public List<Enemy_Combat> enemies_Combat = new List<Enemy_Combat>();
    public UnityEvent OnCombatStart;
    public UnityEvent OnPlayerTurnStart;
    public UnityEvent OnPlayerTurnEnd;
    public UnityEvent OnEnemyTurnStart;
    public UnityEvent OnEnemyTurnEnd;
    public UnityEvent OnCombatEnd;
    private bool isPlayerTurn;
    public void SwitchSide() {isPlayerTurn = !isPlayerTurn;}

    public float expEarnedAfterCombat;
    
    void Start()
    {
        SampleCombat(); // For Test Only
    }

    void SampleCombat()
    {
        PlayerCharacter player = new PlayerCharacter(Resources.Load<PlayerData>("Test/Player"));
        Enemy enemy = new Enemy(Resources.Load<EnemyData>("Test/Enemy"));
        StartCombat(new List<PlayerCharacter>{player}, new List<Enemy>{enemy}, false); 
    }

    void StartCombat(List<PlayerCharacter> playerCharacters, List<Enemy> enemies, bool isFirstStrike)
    {
        this.playerCharacters = playerCharacters;
        this.enemies = enemies;
        
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
        
        PlayerController_Combat.Instance.Init(playerCharacters_Combat);
        CombatUI.Instance.UpdateInfo();
        
        OnCombatStart?.Invoke();
        StartCoroutine(Turns());
    }

    IEnumerator Turns()
    {
        while (true)
        {
            CombatUI.Instance.SwitchTurn(true);
            
            OnPlayerTurnStart?.Invoke();
            yield return new WaitWhile(() => isPlayerTurn);
            OnPlayerTurnEnd?.Invoke();
            
            CombatUI.Instance.SwitchTurn(false);
            
            OnEnemyTurnStart?.Invoke();

            // Placeholder for enemies' actions
            yield return new WaitForSeconds(2f); 
            EndTurn(false);
            
            // TODO: yield return new WaitWhile(() => !isPlayerTurn);
            OnEnemyTurnEnd?.Invoke();
        }
    }

    public void EndTurn(bool isFinishingPlayerTurn)
    {
        if(isPlayerTurn == isFinishingPlayerTurn)
            isPlayerTurn = !isPlayerTurn;
    }

    public void OnNotifiedCharacterDeath(Character_Combat character)
    {
        if (character is Enemy_Combat enemy)
        {
            character.OnCharacterDeath -= OnNotifiedCharacterDeath;
            enemies_Combat.Remove(enemy);
            
            if (enemies_Combat.Count == 0)
            {
                EndCombat();
            }
        }
        else
        {
            // TODO: What do we do when a player character Dies?
        }
    }

    public void TryFlee()
    {
        if (true) // TODO: Flee by chances
        {
            EndCombat();
        }
    }

    public void EndCombat()
    {
        OnCombatEnd?.Invoke();
        
        foreach (var player_combat in playerCharacters_Combat)
            player_combat.OnCharacterDeath -= OnNotifiedCharacterDeath;
        foreach (var enemy_combat in enemies_Combat)
            enemy_combat.OnCharacterDeath -= OnNotifiedCharacterDeath;
        
        // TODO: Reward Player
        foreach (var character in playerCharacters)
        {
            character.GainExperience(expEarnedAfterCombat);
        }
        
        // TODO: Exit Combat Scene
    }
}