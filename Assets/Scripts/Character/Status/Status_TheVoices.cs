using UnityEngine;

public class Status_TheVoices: Status
{
    public CircleRange range;
    public float radius = 2;
    public float ATK_ratio = 1;
    
    public Status_TheVoices(StatusData data, Character_Combat owner, int turns) : base(data, owner, turns)
    {
        owner.OnTurnEnd += DealDamage;
        range = new CircleRange();
        range.radius = radius;
        owner.entity.transform.Find("The Voices Range").GetComponent<SpriteRenderer>().enabled = true;
    }

    public override void Clear()
    {
        base.Clear();
        owner.OnTurnEnd -= DealDamage;
        owner.entity.transform.Find("The Voices Range").GetComponent<SpriteRenderer>().enabled = false;
    }

    public void DealDamage(Character_Combat owner)
    {
        GridManager.Instance.ApplyDamageToTiles(owner, range.GetAllTileCovered(owner), ATK_ratio * owner.ATK);
    }
}