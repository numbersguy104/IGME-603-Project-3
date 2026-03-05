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

    /// <summary>
    /// Extend the duration of a status
    /// </summary>
    /// <param name="extraTurns">The number of turns to be extended</param>
    public virtual void Extend(int extraTurns)
    {
        turnsRemained += extraTurns;
    }

    /// <summary>
    /// Update turn counter. Invoked by the character OnTurnEnd event.
    /// </summary>
    public virtual void NotifyTurnEnd()
    {
        turnsRemained--;
        if (turnsRemained == 0)
            Clear();
    }
    /// <summary>
    /// Remove a status.
    /// </summary>
    public virtual void Clear()
    {
        OnStatusCleared?.Invoke();
    }
}