using UnityEngine;

public class Status_PoweredUp: Status
{
    public float atk_multiplier = 1.25f;
    public Status_PoweredUp(StatusData data, Character_Combat owner, int turns) : base(data, owner, turns)
    {
    }

    public override void Clear()
    {
        base.Clear();
    }
}