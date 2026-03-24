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


    private bool useBlockCheck = false;
    private LayerMask blockingLayers;
    private float skinWidth = 0.05f;

    private CapsuleCollider _capsule;

    /// <summary>
    /// Initializes the Rigidbody component and sets interpolation and rotation constraints.
    /// </summary>
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _capsule = GetComponent<CapsuleCollider>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX |
                          RigidbodyConstraints.FreezeRotationZ |
                          RigidbodyConstraints.FreezeRotationY;
    }

    /// <summary>
    /// Enable or disable forward block checking.
    /// Solid should be false, Ghost should be true.
    /// </summary>
    public void SetBlockCheck(bool active)
    {
        useBlockCheck = active;
    }

    /// <summary>
    /// Sets which layers should block this character.
    /// </summary>
    public void SetBlockingLayers(LayerMask mask)
    {
        blockingLayers = mask;
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

        if (useBlockCheck)
        {
            Vector3 planarTarget = new Vector3(target.x, 0f, target.z);

            if (planarTarget.sqrMagnitude > 0.0001f)
            {
                Vector3 moveDir = planarTarget.normalized;
                float moveDistance = planarTarget.magnitude * Time.fixedDeltaTime;

                if (IsBlocked(moveDir, moveDistance))
                {
                    target.x = 0f;
                    target.z = 0f;
                }
            }
        }

        float maxDelta = acceleration * Time.fixedDeltaTime;
        Vector3 newVel = Vector3.MoveTowards(current, target, maxDelta);

        _rb.linearVelocity = newVel;
    }

    private bool IsBlocked(Vector3 moveDir, float moveDistance)
    {
        Bounds bounds = _capsule.bounds;

        Vector3 center = bounds.center;
        float radius = Mathf.Max(0.01f, _capsule.radius * 0.95f);
        float height = Mathf.Max(_capsule.height * transform.localScale.y, radius * 2f);

        float half = Mathf.Max(0f, height * 0.5f - radius);

        Vector3 point1 = center + Vector3.up * half;
        Vector3 point2 = center - Vector3.up * half;

        return Physics.CapsuleCast(
            point1,
            point2,
            radius,
            moveDir,
            out RaycastHit hit,
            moveDistance + skinWidth,
            blockingLayers,
            QueryTriggerInteraction.Ignore
        );
    }
}
