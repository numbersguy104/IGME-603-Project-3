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
    [SerializeField] private Transform playerStatuses;
    [SerializeField] private Image enemyHPBar;
    [SerializeField] private Transform enemyStatuses;

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
            character.OnStatusUpdated += UpdateCombatInfo;
        }

        foreach (var enemy in enemyCharacters)
        {
            enemy.OnTakeDamage += UpdateCombatInfo;
            enemy.OnStatusUpdated += UpdateCombatInfo;
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
        itemPanelUI.itemList = new List<Item> {new HealingPotion(Resources.Load<ItemData>("Test/HealPotion"))};
        
        // TODO: itemPanelUI.itemList = Inventory.items;
    }

    public void UpdateHP()
    {
        float GetHealthPercentage(Character_Combat character) => character.CurrentHealth / character.MaxHealth;
        playerHPBar.fillAmount = GetHealthPercentage(PlayerController_Combat.Instance.currentCharacter);
        if(CombatManager.Instance.enemies_Combat.Count > 0)
            enemyHPBar.fillAmount = GetHealthPercentage(CombatManager.Instance.enemies_Combat[0]);
        else
            enemyHPBar.fillAmount = 0;
    }

    public void UpdateStatus()
    {
        // Temp
        if (CombatManager.Instance.enemies_Combat.Count == 0)
            return;
        Enemy_Combat enemy = CombatManager.Instance.enemies_Combat[0];
        StatusSlotUI[] statusSlots = enemyStatuses.GetComponentsInChildren<StatusSlotUI>();
        for (int i = 0; i < statusSlots.Length; i++)
        {
            if (i < enemy.statuses.Count)
            {
                statusSlots[i].SetVisible(true);
                statusSlots[i].UpdateStatus(enemy.statuses[i]);
            }
            else
            {
                statusSlots[i].SetVisible(false);
            }
        }
    }

    public void SwitchTurn(bool isPlayerTurn)
    {
        if(isPlayerTurn)
            currentTurn.SetText($"Player's Turn");
        else
            currentTurn.SetText($"Enemy's Turn");
    }
}
