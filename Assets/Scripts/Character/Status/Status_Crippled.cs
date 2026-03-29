using UnityEngine;

public class Status_Crippled: Status
{
    public Status_Crippled(StatusData data, Character_Combat owner, int turns) : base(data, owner, turns)
    {
        owner.maxMovementDistance *= 0.5f;
    }

    public override void Clear()
    {
        base.Clear();
        owner.maxMovementDistance *= 2;
    }
}