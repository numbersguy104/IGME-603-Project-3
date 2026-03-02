using UnityEngine;

public class Status_Weak: Status
{
    public float ratio = 0.75f;

    public Status_Weak(Character_Combat owner, int turns) : base(owner, turns)
    {
        owner.ATK *= ratio;
    }

    public override void Clear()
    {
        base.Clear();
        owner.ATK /= ratio;
    }
}