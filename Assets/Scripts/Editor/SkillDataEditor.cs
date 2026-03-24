using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillData))]
public class SkillDataEditor : Editor
{
    RangeType rangeType;
    public override void OnInspectorGUI()
    {
        SkillData skill = (SkillData)target;

        DrawDefaultInspector();

        if (skill.param == null || !IsSkillTypeMatch(skill.skillType, skill.param))
        {
            skill.param = CreateParam(skill.skillType);
            EditorUtility.SetDirty(skill);
        }
        
        if (skill.range == null || !IsRangeTypeMatch(skill.rangeType, skill.range))
        {
            skill.range = RangeFactory.GetRange(skill.rangeType);
            EditorUtility.SetDirty(skill);
        }

    }

    bool IsSkillTypeMatch(SkillType type, SkillParam param)
    {
        return (type == SkillType.Attack && param is AttackParam) ||
               (type == SkillType.Buff && param is BuffParam);
    }

    SkillParam CreateParam(SkillType type)
    {
        switch (type)
        {
            case SkillType.Attack: return new AttackParam();
            case SkillType.Buff: return new BuffParam();
        }
        return null;
    }
    
    bool IsRangeTypeMatch(RangeType type, Range range)
    {
        return (type == RangeType.Grid && range is GridRange) ||
               (type == RangeType.Sector && range is SectorRange);
    }
}