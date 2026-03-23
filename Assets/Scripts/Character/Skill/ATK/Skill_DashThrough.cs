using UnityEngine;

public class Skill_DashThrough : AttackSkill
{
    public Skill_DashThrough(SkillData skillData) : base(skillData)
    {
    }

    public override void Execute(Character_Combat instigator)
    {
        base.Execute(instigator);
        AttackParam attackParam = skillData.param as AttackParam;

        GridRange range = skillData.range as GridRange;
        Vector2Int tileToMove = instigator.TransformRangeToWorld(new Vector2Int[]{range.relativeCoordinates[^1]})[0];
        if (GridManager.Instance.GetAt(tileToMove.x, tileToMove.y) == null)
        {
            Vector2Int currentTile = GridManager.Instance.PosToGrid(instigator.entity.transform.position);
            GridManager.Instance.Move(currentTile.x, currentTile.y, tileToMove.x, tileToMove.y, true);
        }
    }
}