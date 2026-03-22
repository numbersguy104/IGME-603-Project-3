using UnityEngine;

public class PlayerFieldBounds : MonoBehaviour
{
    public BoxCollider boundsCollider;

    private Bounds bounds;

    void Start()
    {
        if (boundsCollider != null)
            bounds = boundsCollider.bounds;
    }

    void LateUpdate()
    {
        if (boundsCollider == null) return;

        bounds = boundsCollider.bounds;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
        pos.y = transform.position.y;
        pos.z = Mathf.Clamp(pos.z, bounds.min.z, bounds.max.z);

        transform.position = pos;
    }
}
