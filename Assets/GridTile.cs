using System;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    private Renderer renderer;
    private MaterialPropertyBlock mpb;
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void SetHighlight(int type)
    {
        for (int i = 0, j = 1; i < Enum.GetValues(typeof(HighlightType)).Length; i++, j <<= 1)
        {
            renderer.GetPropertyBlock(mpb);
            switch (i)
            {
                case (int)HighlightType.InAttackingRange:
                    mpb.SetFloat("_IsInAtkRange", (type & j) > 0 ? 1 : 0);
                    break;
                case (int)HighlightType.Hovered:
                    mpb.SetFloat("_IsHovered", (type & j) > 0 ? 1 : 0);
                    break;
                case (int)HighlightType.InMovingRange:
                    mpb.SetFloat("_IsInMoveRange", (type & j) > 0 ? 1 : 0);
                    break;
            }
            renderer.SetPropertyBlock(mpb);
        }
    }
}
