using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private readonly List<IInteractable> interactablesInRange = new();

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsPlaying)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");
            TryInteract();
        }
    }

    private void TryInteract()
    {
        for (int i = 0; i < interactablesInRange.Count; i++)
        {
            if (interactablesInRange[i] == null) continue;

            interactablesInRange[i].Interact(gameObject);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("PlayerInteractor entered: " + other.name);

        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        Debug.Log("Found interactable on: " + ((MonoBehaviour)interactable).name);
        if (!interactablesInRange.Contains(interactable))
            interactablesInRange.Add(interactable);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("PlayerInteractor exited: " + other.name);

        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        interactablesInRange.Remove(interactable);
    }
}