using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Combat : Character_Combat
{
    public float movesAvailable;
    public int attacksAvailable;
    public Skill normalAttack;
    public PlayerCharacter_Combat(PlayerCharacter character) : base(character)
    {
        team = Team.Player;
        CombatManager.Instance.OnPlayerTurnStart.AddListener(OnNotifiedTurnStart);
        CombatManager.Instance.OnPlayerTurnEnd.AddListener(OnNotifiedTurnEnd);
        normalAttack = SkillFactory.GetSkill(character.normalAttack);
    }
    ~PlayerCharacter_Combat()
    {
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.OnPlayerTurnStart.RemoveListener(OnNotifiedTurnStart);
            CombatManager.Instance.OnPlayerTurnEnd.RemoveListener(OnNotifiedTurnEnd);
        }
    }

    public override void OnNotifiedTurnStart()
    {
        base.OnNotifiedTurnStart();
        movesAvailable = 1;
        attacksAvailable = 1;
        skillPoint += CombatManager.Instance.SkillPointRegenEveryTurn;
    }

    public void Flee()
    {
        CombatManager.Instance.TryFlee();
    }

    public void UseItem(Item item)
    {
        item.UseInCombat(this);
    }
    
}