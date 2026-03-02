using System;
using System.Collections.Generic;
using TMPro;
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

    public void Init()
    {
        skillPanelUI.UpdateSkillList(PlayerController_Combat.Instance.currentCharacter.skills);
        UpdateInfo();
    }

    public void SwitchCharacter()
    {
        skillPanelUI.UpdateSkillList(PlayerController_Combat.Instance.currentCharacter.skills);
    }
    
    public void UpdateInfo()
    {
        itemPanelUI.itemList = new List<Item> {new Item(Resources.Load<ItemData>("Test/HealPotion"))};
        // TODO: itemPanelUI.itemList = Inventory.items;
        float GetHealthPercentage(Character_Combat character) => character.CurrentHealth / character.MaxHealth;
        playerHPBar.fillAmount = GetHealthPercentage(PlayerController_Combat.Instance.currentCharacter);
        enemyHPBar.fillAmount = GetHealthPercentage(CombatManager.Instance.enemies_Combat[0]);
    }

    public void SwitchTurn(bool isPlayerTurn)
    {
        if(isPlayerTurn)
            currentTurn.SetText($"Player's Turn");
        else
            currentTurn.SetText($"Enemy's Turn");
    }
}
