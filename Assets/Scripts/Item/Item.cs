public class Item
{
    public ItemData itemData;
    
    public Item(ItemData itemData)
    {
        this.itemData = itemData;
    }

    public virtual void UseInField(Character user)
    {
        
    }

    public virtual void UseInCombat(Character_Combat user)
    {
        
    }
}