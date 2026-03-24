using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombatEntity : MonoBehaviour
{
    public Character_Combat character;
    public Image characterImage;
    public Sprite characterSpriteFront;
    public Sprite characterSpriteBack;

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
        
        if(transform.forward == new Vector3(1,0,0) || transform.forward == new Vector3(0,0,-1))
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
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
}
