using UnityEngine;

/// <summary>
/// CameraRigFollow3D is a camera follow script for 3D games.
/// It smoothly follows a target on the XZ plane, with optional look-ahead and boundary constraints.
/// </summary>
public class CameraRigFollow3D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow")]
    public float smooth = 10f;                 // Higher value means more responsive following
    public Vector3 worldOffset = Vector3.zero; // Additional offset in world space

    [Header("Look Ahead (optional)")]
    public bool useLookAhead = false;
    public float lookAheadDistance = 1.5f;     // Offset distance in the movement direction
    public float lookAheadSmooth = 8f;

    [Header("Bounds (optional)")]
    public bool useBounds = false;
    public Vector2 minXZ; // e.g. (-50, -50)
    public Vector2 maxXZ; // e.g. ( 50,  50)

    private Vector3 _prevTargetPos;
    private Vector3 _lookAhead;

    /// <summary>
    /// Initializes the previous target position.
    /// </summary>
    private void Start()
    {
        if (target != null) _prevTargetPos = target.position;
    }

    /// <summary>
    /// Updates the camera position each frame to follow the target smoothly,
    /// with optional look-ahead and boundary constraints.
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate target velocity (on XZ plane)
        Vector3 delta = target.position - _prevTargetPos;
        _prevTargetPos = target.position;

        Vector3 desiredLookAhead = Vector3.zero;
        if (useLookAhead)
        {
            Vector3 dir = new Vector3(delta.x, 0f, delta.z);
            if (dir.sqrMagnitude > 0.0001f)
                desiredLookAhead = dir.normalized * lookAheadDistance;
        }

        _lookAhead = Vector3.Lerp(_lookAhead, desiredLookAhead, 1f - Mathf.Exp(-lookAheadSmooth * Time.deltaTime));

        // Desired position: follow target's XZ, keep rig's own Y (for fixed height logic)
        Vector3 desired = new Vector3(target.position.x, transform.position.y, target.position.z)
                          + worldOffset
                          + _lookAhead;

        // Smooth movement
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-smooth * Time.deltaTime));

        // Boundary constraints (optional)
        if (useBounds)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minXZ.x, maxXZ.x);
            smoothed.z = Mathf.Clamp(smoothed.z, minXZ.y, maxXZ.y);
        }

        transform.position = smoothed;
    }
}
