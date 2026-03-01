using UnityEngine;

public class Health : MonoBehaviour
{
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }

    public void Init(float maxHP)
    {
        MaxHP = maxHP;
        CurrentHP = maxHP;
    }

    public void Damage(float amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        // TODO: add death event
    }
}
