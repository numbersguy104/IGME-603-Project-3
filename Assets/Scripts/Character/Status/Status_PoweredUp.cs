using UnityEngine;

public class Status_PoweredUp: Status
{
    public Status_PoweredUp(StatusData data, Character_Combat owner, int turns) : base(data, owner, turns)
    {
    }

    public override void Clear()
    {
        base.Clear();
    }
}