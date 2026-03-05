using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum Team
{
    Player,
    Enemy
}
public class Character_Combat
{
    #region Attributes
    protected float health;
    protected float maxHealth;
    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;
    protected float mana;
    protected float maxMana;
    public float CurrentMana => mana;
    public float MaxMana => maxMana;

    public float ATK;
    public float Def;
    
    public List<Vector2Int> attackRange;
    #endregion

    public Team team;
    
    public CombatEntity entity;
    
    public List<Skill> skills;
    public List<Status> statuses;

    public Action OnTurnEnd;
    public Action<Character_Combat> OnCharacterDeath;
    public Action OnTakeDamage;
    public Action OnStatusUpdated;
    
    /// <summary>
    /// Create Combat Character Instance from a Character
    /// </summary>
    /// <param name="character">The target character</param>
    public Character_Combat(Character character)
    {
        health = character.CurrentHealth;
        maxHealth = character.MaxHealth;
        mana = character.CurrentMana;
        maxMana = character.MaxMana;
        ATK = character.ATK;
        
        //TODO: Get attackRange from character (according to the weapon?);
        attackRange = new List<Vector2Int> { new(0, 1) }; // For week-1 Test Only
        
        skills =  new List<Skill>();
        foreach (var skillData in character.skillSet)
        {
            Skill skill = null;
            switch (skillData.skillType)
            {
                case SkillType.Attack: 
                    skill = new AttackSkill(skillData);
                    break;
                case SkillType.Defense:
                    skill = new Defense(skillData);
                    break;
                case SkillType.Buff:
                    skill = new BuffSkill(skillData);
                    break;
            }
            if(skill != null)
                skills.Add(skill);
        }
        
        statuses = new List<Status>();
    }
    
    /// <summary>
    /// Invoked when the character's turn ends. Trigger all events that happen at the turn ending phase. After that, update skill CD and status turn counter.
    /// </summary>
    public virtual void OnNotifiedTurnEnd()
    {
        OnTurnEnd?.Invoke();
        
        foreach (Skill skill in skills)
        {
            skill.NotifyTurnEnd();
        }
        for(int i = 0; i < statuses.Count;)
        {
            Status status = statuses[i];
            status.NotifyTurnEnd();
            if (status.TurnsRemained > 0)
                i++;
        }
        
        OnStatusUpdated?.Invoke();
    }

    /// <summary>
    /// Simply cost mana. Technically can also be used to refill mana.
    /// </summary>
    /// <param name="cost">The amount of mana to be cost. Negative if to refill mana</param>
    public virtual void CostMana(float cost)
    {
        mana -= cost;
    }
    
    /// <summary>
    /// Simply Heal the character. Technically can also be used to do damage (Not recommended). Will trigger OnTakeDamage event;
    /// </summary>
    /// <param name="amount">The amount of health to be healed. cannot exceed maximum HP</param>
    public virtual void Healed(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        // VFX?
        
        OnTakeDamage?.Invoke(); // Warning: Currently Used to Trigger UI Update. Maybe at risk
    }

    /// <summary>
    /// Damage the character. Will trigger OnTakeDamage event and OnCharacterDeath if HP <= 0
    /// </summary>
    /// <param name="amount"> The amount of damage to be dealt. </param>
    public virtual void TakeDamage(float amount)
    {
        // TODO: How does the Defense affect the damage?
        
        health = Mathf.Max(health - amount, 0);
        OnTakeDamage?.Invoke();
        // TODO: VFX?
        
        if (health == 0)
        {
            Die();
        }
        
    }

    public virtual void Die()
    {
        OnCharacterDeath?.Invoke(this);
    }

    /// <summary>
    /// Add a status to the character alone with the turns it will last for
    /// </summary>
    /// <param name="statusData"> The status to be applied </param>
    /// <param name="turns">The number of turns it will last for</param>
    public virtual void AddStatus(StatusData statusData, int turns)
    {
        Status existingStatus = statuses.FirstOrDefault(s => s.statusData.statusName == statusData.statusName);
        if (existingStatus != null)
        {
            existingStatus.Extend(turns);
        }
        else
        {
            Status newStatus = StatusFactory.GetStatus(statusData, this, turns);
            statuses.Add(newStatus);
            newStatus.OnStatusCleared += () =>
            {
                statuses.Remove(newStatus);
                OnStatusUpdated?.Invoke();
            };
        }
        
        OnStatusUpdated?.Invoke();
    }

    /// <summary>
    /// Used to transform the local coordinates of skill/attack range into the absolute coordinates in the grid
    /// </summary>
    /// <param name="coors"> The coordinates to be transformed </param>
    /// <returns>The transformed coordinates. </returns>
    // TODO: Add direction
    public Vector2Int[] TransformRangeToWorld(Vector2Int[] coors)
    {
        Vector2Int[] result = new Vector2Int[coors.Length];
        Vector2Int currentCoor = GridManager.Instance.PosToGrid(entity.transform.position);
        int index = 0;
        foreach (var coordinate in coors)
        {
            result[index] = coordinate + currentCoor;
            index++;
        }

        return result;
    }
}