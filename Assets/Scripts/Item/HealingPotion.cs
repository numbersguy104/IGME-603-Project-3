using System;

[Serializable]
public class HealingPotionParam : ItemParam
{
    public float healingAmount;
}
public class HealingPotion : Item
{
    public HealingPotion(ItemData itemData) : base(itemData)
    {
    }

    public override void UseInCombat(Character_Combat user)
    {
        user.Healed(((HealingPotionParam)itemData.param).healingAmount);
    }
    public override void UseInField(Character user)
    {
        user.Healed(((HealingPotionParam)itemData.param).healingAmount);
    }
}