using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utility;

public class CombatUI : SingletonBehavior<CombatUI>
{
    [SerializeField] private TMP_Text currentTurn;
    [SerializeField] private SkillPanelUI skillPanelUI;
    [SerializeField] private ItemPanelUI itemPanelUI;

    public void UpdateInfo()
    {
        skillPanelUI.skillList = PlayerController_Combat.Instance.currentCharacter.skills;
        itemPanelUI.itemList = new List<Item> {new Item(Resources.Load<ItemData>("Test/HealPotion"))};
        // TODO: itemPanelUI.itemList = Inventory.items;
    }

    public void SwitchTurn(bool isPlayerTurn)
    {
        if(isPlayerTurn)
            currentTurn.SetText($"Player's Turn");
        else
            currentTurn.SetText($"Enemy's Turn");
    }
}
