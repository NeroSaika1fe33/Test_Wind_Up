using UnityEngine;

public enum ItemType
{
    None,
    Boost,
    test1,
    test2
}

public class ItemHolder : MonoBehaviour
{
    [Header("Runtime")]
    public ItemType currentItem = ItemType.None;

    private CarEntity carEntity;
    private PartBuffController BuffController;

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
                if (BuffController != null)
                {
                    BuffController.ActivateByItem(ItemType.Boost);
                }
                else
                {
                    Debug.LogWarning("[ItemHolder] TemporaryPartBuffController not found.");
                }
                break;

            case ItemType.test1:
                
                Debug.Log("[ItemHolder]test1 ŽÀ‘•‚µ‚Ä‚È‚¢");
                break;

            case ItemType.test2:
                
                Debug.Log("[ItemHolder] test2 ŽÀ‘•‚µ‚Ä‚È‚¢");
                break;
        }

        currentItem = ItemType.None;
        
    }
}
