using System;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "Player", menuName = "ScriptableObject/Character/Player")]
public class PlayerData : CharacterData
{
    public SkillData normalAttack;
    [Serializable]
    public struct SkillAtLevel
    {
        public SkillData skill;
        public int level;
    }

    public List<SkillAtLevel> skillUnlockedAtLevel;
}