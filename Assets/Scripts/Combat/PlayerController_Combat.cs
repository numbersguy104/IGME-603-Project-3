using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class PlayerController_Combat : SingletonBehavior<PlayerController_Combat>
{
    public List<PlayerCharacter_Combat> teamMembers;
    public PlayerCharacter_Combat currentCharacter;

    public void Init(List<PlayerCharacter_Combat> teamMembers)
    {
        this.teamMembers = teamMembers;
        currentCharacter = teamMembers[0];
    }

    public void Attack()
    {
        Debug.Log("Player Attack");
        GridManager.Instance.ApplyDamageToCells(currentCharacter, currentCharacter.TransformRangeToWorld(currentCharacter.attackRange.ToArray()), currentCharacter.ATK);
        EndPlayerTurn();
    }

    public void UseSkill(Skill skill)
    {
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
        Debug.Log($"Player Use Item: {item.itemData.itemName}");
        item.UseInCombat(currentCharacter);
        // TODO: Remove Item from inventory
    }
    
    public void SwitchCharacter()
    {
        // Assume there's only two characters
        currentCharacter = currentCharacter == teamMembers[0] ? teamMembers[1] : teamMembers[0];
        CombatUI.Instance.UpdateCombatInfo();
    }

    public void EndPlayerTurn()
    {
        CombatManager.Instance.EndTurn(true);
    }
}
