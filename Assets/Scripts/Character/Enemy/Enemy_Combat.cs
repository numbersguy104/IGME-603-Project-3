public class Enemy_Combat: Character_Combat
{
    public Enemy_Combat(Character data) : base(data)
    {
        team = Team.Enemy;
        CombatManager.Instance.OnEnemyTurnEnd?.AddListener(OnNotifiedTurnEnd);
    }

    ~Enemy_Combat()
    {
        CombatManager.Instance?.OnEnemyTurnEnd?.RemoveListener(OnNotifiedTurnEnd);
    }

    public override void OnNotifiedTurnEnd()
    {
        base.OnNotifiedTurnEnd();
        // TODO: Enemy State Machine
    }
    
    public void Die()
    {
        
    }
}