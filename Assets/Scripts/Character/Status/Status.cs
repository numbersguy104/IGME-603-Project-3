using System;

public class Status
{
    public Character_Combat owner;
    public StatusData statusData;
    
    protected int turnsRemained;
    public int TurnsRemained => turnsRemained;
    
    public Action OnStatusCleared;
    
    public Status(StatusData statusData, Character_Combat owner, int turns)
    {
        this.statusData = statusData;
        this.owner = owner;
        turnsRemained = turns;
    }

    public virtual void Extend(int extraTurns)
    {
        turnsRemained += extraTurns;
    }

    public virtual void NotifyTurnEnd()
    {
        turnsRemained--;
        if (turnsRemained == 0)
            Clear();
    }

    public virtual void Clear()
    {
        OnStatusCleared?.Invoke();
    }
}