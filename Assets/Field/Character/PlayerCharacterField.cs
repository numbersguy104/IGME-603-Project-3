using UnityEngine;


/// <summary>
/// Player-controllable character logic used in the field (runtime scene).
/// This component:
/// - Requires a <see cref="CharacterMotor"/> for movement
/// - Requires a <see cref="Health"/> component for HP management
/// - Applies stats from a <see cref="CharacterStatsSO"/> ScriptableObject
/// - Optionally triggers a primary <see cref="AbilityBase"/> when player-controlled
///
/// Intended to be driven by an external controller (e.g., CharacterTeamController / input manager),
/// which calls <see cref="SetPlayerControlled"/>, <see cref="ApplyMove"/>, and <see cref="UsePrimaryAbility"/>.
/// </summary>
[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(Health))]
public class PlayerCharacterField : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private CharacterStatsSO stats;
    [SerializeField] private AbilityBase primaryAbility;

    private CharacterMotor _motor;
    private Health _health;

    /// True if this character currently accepts player input and can use abilities.
    public bool IsPlayerControlled { get; private set; }

    private void Awake()
    {
        _motor = GetComponent<CharacterMotor>();
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        ApplyStats();
    }

    /// <summary>
    /// Applies movement speed and max HP from the assigned stats asset.
    /// Safe to call again at runtime if you swap stats (e.g., upgrades, buffs).
    /// </summary>
    public void ApplyStats()
    {
        if (stats == null) return;

        _motor.moveSpeed = stats.moveSpeed;
        _health.Init(stats.maxHP);
    }

    /// <summary>
    /// Enables or disables player control for this character.
    /// When disabling control, it clears movement input to prevent drifting.
    /// </summary>
    public void SetPlayerControlled(bool active)
    {
        IsPlayerControlled = active;

        if (!active)
            _motor.SetMoveInput(Vector2.zero);
    }

    /// <summary>
    /// Applies movement input to the motor if this character is player-controlled.
    /// </summary>
    public void ApplyMove(Vector2 move)
    {
        if (!IsPlayerControlled) return;
        _motor.SetMoveInput(move);
    }

    /// <summary>
    /// Uses the primary ability if assigned and if this character is player-controlled.
    /// </summary>
    public void UsePrimaryAbility()
    {
        if (!IsPlayerControlled) return;
        if (primaryAbility != null) primaryAbility.Use();
    }
}