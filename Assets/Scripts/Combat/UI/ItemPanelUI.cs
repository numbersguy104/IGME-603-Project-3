using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemPanelUI : MonoBehaviour
{
    public List<Item> itemList;
    private void OnEnable()
    {
        // TODO: Temp Test Only
        Transform itemButtons = transform.Find("Items");
        for (int i = 0; i < itemButtons.childCount; i++)
            itemButtons.GetChild(i).GetComponent<TempItemButton>().item = itemList[0];
    }

    public void UpdateItemList(List<Item> itemList)
    {
        this.itemList = itemList;
        // TODO:
        // Transform itemButtons = transform.Find("Items");
        // for (int i = 0; i < itemButtons.childCount; i++)
        //     Destroy(itemButtons.GetChild(i).gameObject);
        // foreach (Item item in itemList)
        //     CreateButton(item);
    }

    public void CreateButton(Item item)
    {
        // TODO
    }
}