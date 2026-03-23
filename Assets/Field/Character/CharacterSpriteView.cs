using UnityEngine;

public class CharacterSpriteView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [Header("Settings")]
    [SerializeField] private float switchThreshold = 0.1f;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateFacing(Vector2 moveInput)
    {
        if (spriteRenderer == null) return;

        // W / forward -> use back sprite
        if (moveInput.y > switchThreshold)
        {
            if (backSprite != null)
                spriteRenderer.sprite = backSprite;
        }
        // S / backward or idle -> use front sprite
        else if (moveInput.y < -switchThreshold || moveInput.sqrMagnitude <= switchThreshold * switchThreshold)
        {
            if (frontSprite != null)
                spriteRenderer.sprite = frontSprite;
        }

        if (moveInput.x > switchThreshold)
        {
            spriteRenderer.flipX = true; // to right
        }
        else if (moveInput.x < -switchThreshold)
        {
            spriteRenderer.flipX = false;  // to left
        }
    }
}