using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;

public class CombatUI : SingletonBehavior<CombatUI>
{
    [SerializeField] private TMP_Text currentTurn;
    [SerializeField] private Button attackButton;
    private UnityAction attackAction = () => PlayerController_Combat.Instance.Attack();
    [SerializeField] private SkillPanelUI skillPanelUI;
    [SerializeField] private ItemPanelUI itemPanelUI;
    [SerializeField] private Image playerHPBar;
    [SerializeField] private Image enemyHPBar;

    private void OnEnable()
    {
        attackButton.onClick.AddListener(attackAction);
    }

    private void OnDisable()
    {
        attackButton.onClick.RemoveListener(attackAction);
    }

    public void Init(List<PlayerCharacter_Combat> playerCharacters, List<Enemy_Combat> enemyCharacters)
    {
        UpdateSkillList();
        UpdateItems();
        UpdateCombatInfo();
        foreach (var character in playerCharacters)
        {
            character.OnTakeDamage += UpdateCombatInfo;   
        }

        foreach (var enemy in enemyCharacters)
        {
            enemy.OnTakeDamage += UpdateCombatInfo;
        }
    }

    public void UpdateSkillList()
    {
        skillPanelUI.UpdateSkillList(PlayerController_Combat.Instance.currentCharacter.skills);
    }
    
    public void UpdateCombatInfo()
    {
        UpdateHP();
        UpdateStatus();
    }

    public void UpdateItems()
    {
        //Temp
        itemPanelUI.itemList = new List<Item> {new Item(Resources.Load<ItemData>("Test/HealPotion"))};
        
        // TODO: itemPanelUI.itemList = Inventory.items;
    }

    public void UpdateHP()
    {
        float GetHealthPercentage(Character_Combat character) => character.CurrentHealth / character.MaxHealth;
        playerHPBar.fillAmount = GetHealthPercentage(PlayerController_Combat.Instance.currentCharacter);
        enemyHPBar.fillAmount = GetHealthPercentage(CombatManager.Instance.enemies_Combat[0]);
    }

    public void UpdateStatus()
    {
        // TODO
    }

    public void SwitchTurn(bool isPlayerTurn)
    {
        if(isPlayerTurn)
            currentTurn.SetText($"Player's Turn");
        else
            currentTurn.SetText($"Enemy's Turn");
    }
}
