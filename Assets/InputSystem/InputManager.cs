using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Centralized input bridge for the player's field controls (Unity Input System).
/// This component:
/// - Listens to input actions from the generated <see cref="PlayerInputSystem"/> wrapper
/// - Stores the current movement Vector2
/// - Sends movement input to the currently active character via <see cref="CharacterTeamController"/>
/// - Triggers character switching when the SwitchCharacter action is performed
///
/// Notes:
/// - Assign <see cref="characterTeam"/> in the Inspector.
/// - The Input Actions asset must contain an action map named "Player_Field"
///   with actions "Move" (Vector2) and "SwitchCharacter" (Button), matching your generated class.
/// </summary>
public class InputManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterTeamController characterTeam;

    private PlayerInputSystem _input;
    private Vector2 _move;

    private void Awake()
    {
        _input = new PlayerInputSystem();

        // Cache move input from the Input System
        _input.Player_Field.Move.performed += ctx => _move = ctx.ReadValue<Vector2>();
        _input.Player_Field.Move.canceled += _ => _move = Vector2.zero;

        // Switch to the other character when requested
        _input.Player_Field.SwitchCharacter.performed += _ =>
        {
            if (characterTeam != null)
                characterTeam.RequestSwitch();
        };
    }

    private void OnEnable()
    {
        _input.Player_Field.Enable();
    }

    private void OnDisable()
    {
        _input.Player_Field.Disable();
    }

    private void Update()
    {
        // Drive the active character each frame using the last cached move input
        if (characterTeam == null || characterTeam.Active == null) return;
        characterTeam.Active.ApplyMove(_move);
    }
}