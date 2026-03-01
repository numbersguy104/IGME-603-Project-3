using UnityEngine;

/// <summary>
/// CharacterMotor handles character movement using Rigidbody physics.
/// It supports configurable move speed and acceleration, and processes movement input.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CharacterMotor : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;
    public float acceleration = 40f;

    private Rigidbody _rb;
    private Vector3 _desiredVelocity;

    /// <summary>
    /// Initializes the Rigidbody component and sets interpolation and rotation constraints.
    /// </summary>
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX |
                          RigidbodyConstraints.FreezeRotationZ |
                          RigidbodyConstraints.FreezeRotationY;
    }

    /// <summary>
    /// Sets the desired movement direction and speed based on input.
    /// </summary>
    /// <param name="input">Movement input vector (x: right/left, y: forward/backward).</param>
    public void SetMoveInput(Vector2 input)
    {
        // x is right and left, y is forward and backward
        var dir = new Vector3(input.x, 0f, input.y);
        dir = Vector3.ClampMagnitude(dir, 1f);
        _desiredVelocity = dir * moveSpeed;
    }

    /// <summary>
    /// Updates the Rigidbody's velocity each physics frame to move the character smoothly.
    /// </summary>
    private void FixedUpdate()
    {
        Vector3 current = _rb.linearVelocity;
        Vector3 target = new Vector3(_desiredVelocity.x, current.y, _desiredVelocity.z);

        float maxDelta = acceleration * Time.fixedDeltaTime;
        Vector3 newVel = Vector3.MoveTowards(current, target, maxDelta);

        _rb.linearVelocity = newVel;
    }
}
