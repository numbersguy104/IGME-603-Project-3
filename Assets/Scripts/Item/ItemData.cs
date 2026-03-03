using System;
using UnityEngine;

[Serializable] public class ItemParam{}

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public bool canBeUsedInField;
    public bool canBeUsedInCombat;
    
    [SerializeReference] public ItemParam param;
}
