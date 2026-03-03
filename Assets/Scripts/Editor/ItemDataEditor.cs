using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemData item = (ItemData)target;

        DrawDefaultInspector();

        if (item.param == null || !IsMatch(item, item.param))
        {
            item.param = CreateParam(item);
            EditorUtility.SetDirty(item);
        }
    }

    bool IsMatch(ItemData item, ItemParam param)
    {
        return (item.itemName == "HealingPotion" && param is HealingPotionParam);
    }

    ItemParam CreateParam(ItemData item)
    {
        switch (item.itemName)
        {
            case "HealingPotion": return new HealingPotionParam();
        }
        return null;
    }
}