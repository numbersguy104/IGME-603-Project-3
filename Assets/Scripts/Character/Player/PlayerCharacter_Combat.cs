using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Combat : Character_Combat
{
    public int movesAvailable;
    public int attacksAvailable;
    public PlayerCharacter_Combat(Character data) : base(data)
    {
        team = Team.Player;
        CombatManager.Instance.OnPlayerTurnStart.AddListener(OnNotifiedTurnStart);
        CombatManager.Instance.OnPlayerTurnEnd.AddListener(OnNotifiedTurnEnd);
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