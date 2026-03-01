using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera _cam;

    private void Awake() => _cam = Camera.main;

    private void LateUpdate()
    {
        if (_cam == null) return;

        Vector3 e = _cam.transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, e.y, 0f);
    }
}
