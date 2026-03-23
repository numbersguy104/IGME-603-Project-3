using UnityEngine;

public class CharacterSpriteView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float switchThreshold = 0.1f;

    private string currentState;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void UpdateFacing(Vector2 moveInput)
    {
        if (spriteRenderer == null || animator == null) return;

        animator.SetFloat("Speed", moveInput.magnitude);
        animator.SetFloat("MoveZ", moveInput.y);

        // Right
        if (moveInput.x < - switchThreshold)
        {
            spriteRenderer.flipX = false;
        }
        // Left
        else if (moveInput.x > switchThreshold)
        {
            spriteRenderer.flipX = true;
        }
    }
    
}