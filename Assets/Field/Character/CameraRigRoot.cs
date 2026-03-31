using UnityEngine;

/// <summary>
/// CameraRigFollow3D smoothly follows a target in 3D space, maintaining a fixed offset and optionally locking camera rotation.
/// Useful for 2.5D or side-scrolling games where the camera should not rotate with the target.
/// </summary>
public class CameraRigFollow3D : MonoBehaviour
{
    /// <summary>
    /// The target transform for the camera to follow.
    /// </summary>
    public Transform target;

    [Header("Follow")]
    /// <summary>
    /// Position smoothing factor. Higher values result in more responsive following.
    /// </summary>
    public float smoothPos = 10f;

    /// <summary>
    /// Offset from the target in world space.
    /// </summary>
    public Vector3 worldOffset = new Vector3(0f, 6f, -6f);

    [Header("Camera Bounds")]
    public BoxCollider cameraBoundsCollider;

    [Header("Lock Rotation")]
    /// <summary>
    /// If true, the camera's rotation is locked to its initial value.
    /// </summary>
    public bool lockRotation = true;

    private Quaternion _lockedRot;

    private bool _followEnabled = true;

    /// <summary>
    /// Caches the initial rotation to maintain a fixed camera direction if rotation locking is enabled.
    /// </summary>
    private void Awake()
    {
        _lockedRot = transform.rotation; // Cache the initial rotation to keep the camera facing a fixed direction
    }

    /// <summary>
    /// Updates the camera's position each frame to smoothly follow the target.
    /// Optionally locks the camera's rotation.
    /// </summary>
    private void LateUpdate()
    {
        if (!_followEnabled) return;
        if (target == null) return;

        Vector3 desiredPos = GetDesiredPosition();

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            1f - Mathf.Exp(-smoothPos * Time.deltaTime)
        );

        if (lockRotation)
            transform.rotation = _lockedRot;
    }

    public void SetFollowEnabled(bool enabled)
    {
        _followEnabled = enabled;
    }

    public void SnapToTargetNow()
    {
        if (target == null) return;

        transform.position = GetDesiredPosition();

        if (lockRotation)
            transform.rotation = _lockedRot;
    }

    private Vector3 GetDesiredPosition()
    {
        Vector3 desiredPos = target.position + worldOffset;

        if (cameraBoundsCollider != null)
        {
            Bounds camBounds = cameraBoundsCollider.bounds;

            desiredPos.x = Mathf.Clamp(desiredPos.x, camBounds.min.x, camBounds.max.x);
            desiredPos.z = Mathf.Clamp(desiredPos.z, camBounds.min.z, camBounds.max.z);
        }

        return desiredPos;
    }
}