using UnityEngine;

/// <summary>
/// CameraFieldBounds smoothly follows a target while keeping the camera position within specified bounds.
/// The camera maintains a fixed offset and locked rotation.
/// </summary>
public class CameraFieldBounds : MonoBehaviour
{
    /// <summary>
    /// The target transform for the camera to follow.
    /// </summary>
    public Transform target;

    /// <summary>
    /// Offset from the target in world space.
    /// </summary>
    public Vector3 offset = new Vector3(0f, 6f, -6f);

    /// <summary>
    /// Smoothing factor for camera movement.
    /// </summary>
    public float smoothSpeed = 8f;

    /// <summary>
    /// BoxCollider that defines the camera's movement bounds.
    /// </summary>
    public BoxCollider cameraBoundsCollider;

    private Bounds camBounds;
    private Quaternion lockedRotation;

    /// <summary>
    /// Initializes the locked rotation and camera bounds.
    /// </summary>
    void Start()
    {
        lockedRotation = transform.rotation;

        if (cameraBoundsCollider != null)
            camBounds = cameraBoundsCollider.bounds;
    }

    /// <summary>
    /// Updates the camera's position each frame to follow the target within the defined bounds and maintains locked rotation.
    /// </summary>
    void LateUpdate()
    {
        if (target == null) return;
        if (cameraBoundsCollider == null) return;

        camBounds = cameraBoundsCollider.bounds;

        Vector3 desiredPos = target.position + offset;

        desiredPos.x = Mathf.Clamp(desiredPos.x, camBounds.min.x, camBounds.max.x);
        desiredPos.y = Mathf.Clamp(desiredPos.y, camBounds.min.y, camBounds.max.y);
        desiredPos.z = Mathf.Clamp(desiredPos.z, camBounds.min.z, camBounds.max.z);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            1f - Mathf.Exp(-smoothSpeed * Time.deltaTime)
        );

        transform.rotation = lockedRotation;
    }
}
