using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillData))]
public class SkillDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SkillData skill = (SkillData)target;

        DrawDefaultInspector();

        if (skill.param == null || !IsMatch(skill.skillType, skill.param))
        {
            skill.param = CreateParam(skill.skillType);
            EditorUtility.SetDirty(skill);
        }
    }

    bool IsMatch(SkillType type, SkillParam param)
    {
        return (type == SkillType.Attack && param is AttackParam) ||
               (type == SkillType.Defense && param is DefenseParam) ||
               (type == SkillType.Buff && param is BuffParam);
    }

    SkillParam CreateParam(SkillType type)
    {
        switch (type)
        {
            case SkillType.Attack: return new AttackParam();
            case SkillType.Defense: return new DefenseParam();
            case SkillType.Buff: return new BuffParam();
        }
        return null;
    }
}