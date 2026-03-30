using System;
using UnityEngine;

public class Status_Burnt : Status
{
    public float dmg = 10f;
    public Action<Character_Combat, float> DamagedFromBurnt = (owner, dmg) => owner.TakeDamage(dmg);

    private Action<Character_Combat> turnEndCallback;

    public Status_Burnt(StatusData data, Character_Combat owner, int turns) : base(data, owner, turns)
    {
        turnEndCallback = (x) => DamagedFromBurnt(x, dmg);
        owner.OnTurnEnd += turnEndCallback;
    }

    public override void Clear()
    {
        base.Clear();
        owner.OnTurnEnd -= turnEndCallback;
    }
}