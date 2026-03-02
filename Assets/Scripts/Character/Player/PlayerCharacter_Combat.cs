using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Combat : Character_Combat
{
    public PlayerCharacter_Combat(Character data) : base(data)
    {
        team = Team.Player;
        CombatManager.Instance.OnPlayerTurnEnd.AddListener(OnNotifiedTurnEnd);
    }
    ~PlayerCharacter_Combat()
    {
        if(CombatManager.Instance !=null)
            CombatManager.Instance.OnPlayerTurnEnd.RemoveListener(OnNotifiedTurnEnd);
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