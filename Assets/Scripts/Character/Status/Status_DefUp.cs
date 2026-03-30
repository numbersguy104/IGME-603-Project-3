using UnityEngine;

public class Status_DefUp: Status
{
    private float Def_increment = 10;
    public Status_DefUp(StatusData data, Character_Combat owner, int turns) : base(data, owner, turns)
    {
        owner.DEF += Def_increment;
    }

    public override void Clear()
    {
        base.Clear();
        owner.DEF -= Def_increment;
    }
}