using System;
using UnityEngine;
using UnityEngine.UI;

public class CombatEntity : MonoBehaviour
{
    public Character_Combat character;
    public Image characterImage;
    public Sprite characterSpriteFront;
    public Sprite characterSpriteBack;

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
}
