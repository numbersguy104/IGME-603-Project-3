using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages a two-character team (Solid and Ghost) where only one character is active and player-controlled at a time.
/// Switching will:
/// - Copy the active character's position/rotation (and optional Rigidbody velocity) to the inactive one
/// - Transfer player control
/// - Deactivate the previous character
///
/// Typical usage:
/// - Put both character instances (solid and ghost) in the scene
/// - Assign them in the Inspector
/// - Call <see cref="SwitchToNext"/> from input (e.g., a key/button)
/// - Provide a camera follow pivot Transform that your camera rig follows
/// </summary>
public class CharacterTeamController : MonoBehaviour
{
    [Header("Scene Character Instances")]
    [SerializeField] private PlayerCharacterField solid;
    [SerializeField] private PlayerCharacterField ghost;

    [Header("Start")]
    [SerializeField] private bool startAsSolid = true;

    /// A transform used as the camera follow target. Keep it aligned to the active character.
    public Transform cameraFollowPivot;


    /// The character that is currently enabled and player-controlled.
    public PlayerCharacterField Active { get; private set; }

    public PlayerCharacterField Inactive { get; private set; }

    /// True if the current active character is the solid character.

    /// The character that is currently disabled and NOT player-controlled.
    private bool _switchRequested;
    private bool _justSwitched;
    private bool _isInitialized;

    public bool IsActiveSolid => Active == solid;

    /// <summary>
    /// Initializes the team controller and attempts to restore state from a battle return if applicable.
    /// </summary>
    private void Awake()
    {
        if (solid == null || ghost == null)
        {
            Debug.LogError("CharacterTeamController: solid/ghost reference is missing.");
            return;
        }
    }

    /// <summary>
    /// Initializes the team state on scene start
    /// </summary>
    private void Start()
    {
        InitializeDefaultState();
    }

    /// <summary>
    /// Initializes the default active/inactive character state and camera pivot.
    /// </summary>
    private void InitializeDefaultState()
    {
        if (_isInitialized) return;

        Active = startAsSolid ? solid : ghost;
        Inactive = startAsSolid ? ghost : solid;

        ActivateOnly(Active, Inactive);

        if (cameraFollowPivot != null && Active != null)
            cameraFollowPivot.position = Active.transform.position;

        _isInitialized = true;
    }

    /// <summary>
    /// Explicit entry point for field restore. Only the restore coordinator should call this.
    /// </summary>
    public void ApplyRestoredFieldState(bool useSolidAsActive, Vector3 position)
    {
        if (solid == null || ghost == null)
        {
            Debug.LogError("CharacterTeamController: solid/ghost reference is missing.");
            return;
        }

        SetCharacterPosition(solid, position);
        SetCharacterPosition(ghost, position);

        Active = useSolidAsActive ? solid : ghost;
        Inactive = useSolidAsActive ? ghost : solid;

        ActivateOnly(Active, Inactive);

        if (cameraFollowPivot != null && Active != null)
            cameraFollowPivot.position = Active.transform.position;

        _isInitialized = true;
        _justSwitched = true;

        Debug.Log(
            $"[CharacterTeamController] ApplyRestoredFieldState | useSolidAsActive={useSolidAsActive} | position={position} | active={Active.name}"
        );
    }

    public void SetInputEnabled(bool enabled)
    {
        if (solid != null)
            solid.SetPlayerControlled(enabled && Active == solid);

        if (ghost != null)
            ghost.SetPlayerControlled(enabled && Active == ghost);
    }

    /// <summary>
    /// Requests a character switch to be processed in the next FixedUpdate.
    /// </summary>
    public void RequestSwitch()
    {
        _switchRequested = true;
    }

    /// <summary>
    /// Handles character switching logic in the physics update loop.
    /// </summary>
    private void FixedUpdate()
    {
        if (!_switchRequested) return;
        _switchRequested = false;

        SwitchToNext(); // Make sure to get the current character position
    }

    /// <summary>
    /// Switches control to the other character.
    /// The newly activated character will be moved to the previous active character's position/rotation,
    /// and will optionally inherit Rigidbody linear velocity (if present).
    /// </summary>
    public void SwitchToNext()
    {
        if (Active == null || Inactive == null) return;

        // 1) Capture current active character state
        Vector3 pos;
        Quaternion rot;
        Vector3 vel = Vector3.zero;

        if (Active.TryGetComponent<Rigidbody>(out var rbA))
        {
            vel = rbA.linearVelocity;
        }

        pos = Active.transform.position;
        rot = Active.transform.rotation;

        // 2) Swap roles: newActive becomes the previously inactive character
        PlayerCharacterField newActive = Inactive;
        PlayerCharacterField newInactive = Active;

        // 3) Enable the new character and place it at the old character's transform
        newActive.gameObject.SetActive(true);

        // If the new character has a Rigidbody, copy velocity and clear angular velocity
        if (newActive.TryGetComponent<Rigidbody>(out var rbB))
        {
            rbB.position = pos;
            rbB.rotation = rot;
            rbB.linearVelocity = vel;
            rbB.angularVelocity = Vector3.zero;
        }
        else
        {
            newActive.transform.SetPositionAndRotation(pos, rot);
        }

        // 4) Transfer player control
        Active.SetPlayerControlled(false);
        newActive.SetPlayerControlled(true);

        // 5) Disable the old character
        newInactive.gameObject.SetActive(false);

        // 6) Update references
        Active = newActive;
        Inactive = newInactive;

        // Mark that we just switched, so camera pivot can hard-sync once in LateUpdate
        _justSwitched = true;
    }

    /// <summary>
    /// Restore the team state after returning from combat.
    /// This places both characters at the same saved field position,
    /// then re-applies which character should be active.
    /// </summary>
    /// 

    private void SetCharacterPosition(PlayerCharacterField character, Vector3 position)
    {
        if (character == null) return;

        if (character.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.position = position;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            character.transform.position = position;
        }
    }


    /// <summary>
    /// Ensures that only the given active character is enabled and player-controlled,
    /// while the inactive one is disabled and not player-controlled.
    /// </summary>
    private void ActivateOnly(PlayerCharacterField active, PlayerCharacterField inactive)
    {
        active.gameObject.SetActive(true);
        inactive.gameObject.SetActive(false);

        active.SetPlayerControlled(true);
        inactive.SetPlayerControlled(false);
    }

    private void LateUpdate()
    {
        if (cameraFollowPivot == null || Active == null) return;

        if (_justSwitched)
        {
            cameraFollowPivot.position = Active.transform.position;
            _justSwitched = false;
            return;
        }

        cameraFollowPivot.position = Active.transform.position;
    }
}

