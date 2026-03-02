using System;
using UnityEngine;

public class Status_Burnt : Status
{
    public float dmg = 10f;
    public Action<Character_Combat, float> DamagedFromBurnt = (owner, dmg) => owner.TakeDamage(dmg);

    private Action turnEndCallback;

    public Status_Burnt(Character_Combat owner, int turns) : base(owner, turns)
    {
        turnEndCallback = () => DamagedFromBurnt(owner, dmg);
        owner.OnTurnEnd += turnEndCallback;
    }

    public override void Clear()
    {
        base.Clear();
        owner.OnTurnEnd -= turnEndCallback;
    }
}