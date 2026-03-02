using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class PlayerController_Combat : SingletonBehavior<PlayerController_Combat>
{
    public List<PlayerCharacter_Combat> teamMembers;
    public PlayerCharacter_Combat currentCharacter;

    private bool isSelectingTile = false;
    private Vector2Int selectedTile;
    public void Init(List<PlayerCharacter_Combat> teamMembers)
    {
        this.teamMembers = teamMembers;
        currentCharacter = teamMembers[0];
    }

    private void Update()
    {
        if (isSelectingTile)
        {
            selectedTile = GridManager.Instance.GetHoveredTile();
            if (selectedTile.x >= 0 && Input.GetMouseButtonDown(0))
                MoveTo(selectedTile);
        }
            
    }

    public void Attack()
    {
        if (!CombatManager.Instance.isPlayerTurn)
            return;
        Debug.Log("Player Attack");
        GridManager.Instance.ApplyDamageToCells(currentCharacter, currentCharacter.TransformRangeToWorld(currentCharacter.attackRange.ToArray()), currentCharacter.ATK);
        EndPlayerTurn();
    }

    public void UseSkill(Skill skill)
    {
        if (!CombatManager.Instance.isPlayerTurn)
            return;
        Debug.Log($"Player Use Skill: {skill.skillData.skillName}");
        switch (skill)
        {
            case AttackSkill attack:
                attack.Execute(currentCharacter);
                EndPlayerTurn();
                break;
            case Defense defense:
                defense.Execute(currentCharacter);
                break;
            case BuffSkill buff:
                buff.Execute(currentCharacter);
                break;
        }
    }

    public void UseItem(Item item)
    {
        if (!CombatManager.Instance.isPlayerTurn)
            return;
        Debug.Log($"Player Use Item: {item.itemData.itemName}");
        item.UseInCombat(currentCharacter);
        // TODO: Remove Item from inventory
    }

    public void PrepareMove()
    {
        if (!CombatManager.Instance.isPlayerTurn)
            return;
        isSelectingTile = true;
    }

    public void MoveTo(Vector2Int tile)
    {
        Vector2Int currentPosition = GridManager.Instance.PosToGrid(currentCharacter.entity.transform.position);
        GridManager.Instance.Move(currentPosition.x, currentPosition.y, selectedTile.x, selectedTile.y, true);
        EndPlayerTurn();
    }
    
    public void SwitchCharacter()
    {
        // Assume there's only two characters
        currentCharacter = currentCharacter == teamMembers[0] ? teamMembers[1] : teamMembers[0];
        CombatUI.Instance.UpdateCombatInfo();
    }

    public void EndPlayerTurn()
    {
        isSelectingTile = false;
        CombatManager.Instance.EndTurn(true);
    }
}
