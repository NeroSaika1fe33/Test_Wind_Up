using UnityEngine;

public enum ItemType
{
    None,
    Boost,
    Missile,
    Banana
}

public class ItemHolder : MonoBehaviour
{
    [Header("Runtime")]
    public ItemType currentItem = ItemType.None;

    public bool HasItem => currentItem != ItemType.None;

    public void GiveItem(ItemType item)
    {
        currentItem = item;
       
        Debug.Log($"[ItemHolder] Got item: {currentItem}");
    }

    public void UseItem()
    {
        if (!HasItem) return;

        
        Debug.Log($"[ItemHolder] Use item: {currentItem}");

        switch (currentItem)
        {
            case ItemType.Boost:
                
                break;

            case ItemType.Missile:
               
                break;

            case ItemType.Banana:
                
                break;
        }

        currentItem = ItemType.None;
        
    }
}
