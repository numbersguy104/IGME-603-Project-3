using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue owner;

    private void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<Dialogue>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner == null) return;
        owner.NotifyTriggerEnter(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (owner == null) return;
        owner.NotifyCollisionEnter(collision.gameObject);
    }
}