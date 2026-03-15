using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    [SerializeField] private NPCDialogue owner;

    private void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<NPCDialogue>();
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