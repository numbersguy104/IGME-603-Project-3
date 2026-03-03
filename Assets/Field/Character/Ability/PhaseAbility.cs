using UnityEngine;

public class PhaseAbility : AbilityBase
{
    [SerializeField] private int ghostLayer;
    [SerializeField] private int solidLayer;
    [SerializeField] private float duration = 1.5f;

    private bool _phasing;

    public override void Use()
    {
        gameObject.layer = ghostLayer;
        //if (_phasing) return;
        //StartCoroutine(DoPhase());
    }

    private System.Collections.IEnumerator DoPhase()
    {
        _phasing = true;
        gameObject.layer = ghostLayer;
        yield return new WaitForSeconds(duration);
        gameObject.layer = solidLayer;
        _phasing = false;
    }
}
