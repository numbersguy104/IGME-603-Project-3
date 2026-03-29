using UnityEngine;

public class DialogueSceneActor : MonoBehaviour
{
    [Header("Actor Sprites")]
    [SerializeField] private GameObject hugoSprite;
    [SerializeField] private GameObject tenetSprite;

    [Header("Optional Facing")]
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private Camera targetCamera;

    private void Awake()
    {
        Hide();
    }

    private void LateUpdate()
    {
        if (!faceCamera) return;

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null) return;

        Vector3 forward = targetCamera.transform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude > 0.0001f)
            transform.forward = forward;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (hugoSprite != null) hugoSprite.SetActive(true);
        if (tenetSprite != null) tenetSprite.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}