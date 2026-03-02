using UnityEngine;

public class Status_Weak: Status
{
    public float ratio = 0.75f;

    public Status_Weak(StatusData data, Character_Combat owner, int turns) : base(data, owner, turns)
    {
        owner.ATK *= ratio;
    }

    public override void Clear()
    {
        base.Clear();
        owner.ATK /= ratio;
    }
}