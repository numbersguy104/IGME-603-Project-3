using UnityEngine;

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

    /// The character that is currently disabled and NOT player-controlled.

    public PlayerCharacterField Inactive { get; private set; }

    private void Start()
    {
        if (solid == null || ghost == null)
        {
            Debug.LogError("CharacterTeamController: solid/ghost reference is missing.");
            return;
        }

        // Initialize Active / Inactive
        if (startAsSolid)
        {
            Active = solid;
            Inactive = ghost;
        }
        else
        {
            Active = ghost;
            Inactive = solid;
        }

        // Ensure only one character is enabled
        ActivateOnly(Active, Inactive);
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
        Vector3 pos = Active.transform.position;
        Quaternion rot = Active.transform.rotation;

        Vector3 vel = Vector3.zero;
        if (Active.TryGetComponent<Rigidbody>(out var rbA))
            vel = rbA.linearVelocity;

        // 2) Swap roles: newActive becomes the previously inactive character
        PlayerCharacterField newActive = Inactive;
        PlayerCharacterField newInactive = Active;

        // 3) Enable the new character and place it at the old character's transform
        newActive.gameObject.SetActive(true);
        newActive.transform.SetPositionAndRotation(pos, rot);

        // If the new character has a Rigidbody, copy velocity and clear angular velocity
        if (newActive.TryGetComponent<Rigidbody>(out var rbB))
        {
            rbB.linearVelocity = vel;
            rbB.angularVelocity = Vector3.zero;
        }

        // 4) Transfer player control
        Active.SetPlayerControlled(false);
        newActive.SetPlayerControlled(true);

        // 5) Disable the old character
        newInactive.gameObject.SetActive(false);

        // 6) Update references
        Active = newActive;
        Inactive = newInactive;
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
        // Keep the camera follow pivot aligned with the active character each frame
        if (cameraFollowPivot != null && Active != null)
            cameraFollowPivot.position = Active.transform.position;
    }
}

