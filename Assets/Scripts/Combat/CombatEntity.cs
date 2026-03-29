using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombatEntity : MonoBehaviour
{
    public Character_Combat character;
    public SpriteRenderer characterImage;
    public Sprite characterSpriteFront;
    public Sprite characterSpriteBack;
    public SpriteRenderer rangeRenderer;
    public Color moveRangeColor;
    public Color attackRangeColor;
    public ParticleSystem attackedFX;

    public float moveSpeed = 1;

    private void LateUpdate()
    {
        Turn();
    }

    public void Turn()
    {
        Camera camera = Camera.main;
        characterImage.sprite = Vector3.Dot(camera.transform.forward, transform.forward) > 0 ? characterSpriteBack : characterSpriteFront; 
        characterImage.transform.rotation =  camera.transform.rotation;

        if (transform.forward.x > 0.707f || transform.forward.z < -0.707f)
            characterImage.flipX = true;
        else
            characterImage.flipX = false;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        StopCoroutine(nameof(Move));
        StartCoroutine(nameof(Move), targetPosition);
    }

    IEnumerator Move(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            PlayerController_Combat.Instance.currentCharacter.movesAvailable -= (moveSpeed * Time.deltaTime) / PlayerController_Combat.Instance.currentCharacter.maxMovementDistance;
            if (PlayerController_Combat.Instance.currentCharacter.movesAvailable < 0.05f)
                PlayerController_Combat.Instance.currentCharacter.movesAvailable = 0;
            CombatUI.Instance.UpdateCombatInfo();
            yield return null;
        }
    }

    public void DisplayRange(float radius, RoundRangeHighlight highlightType)
    {
        rangeRenderer.enabled = true;
        rangeRenderer.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        switch (highlightType)
        {
            case RoundRangeHighlight.Move:
                rangeRenderer.color = moveRangeColor;
                break;
            case RoundRangeHighlight.Attack:
                rangeRenderer.color = attackRangeColor;
                break;
        }
    }

    public void HideRange()
    {
        rangeRenderer.enabled = false;
    }

    public void EntityGetAttacked()
    {
        var animator = gameObject.GetComponent<Animator>();
        if (animator != null && attackedFX != null)
        {
            animator.Play("Attacked");
            attackedFX.Play();
        }
    }
}

public enum RoundRangeHighlight
{
    Move,
    Attack
}
