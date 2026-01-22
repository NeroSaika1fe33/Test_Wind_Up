using System.Diagnostics;
using System.Linq.Expressions;
using UnityEngine;

public class ObjectContact : MonoBehaviour
{
    PartsContainer _PartsContainer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(this.TryGetComponent<PartsContainer>(out var component))
        {
            _PartsContainer = component;
        }
        else
        {
            UnityEngine.Debug.LogError("PartsContainerスクリプトの認識に失敗しました。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //otherにObject_Typeが指定されていれば接触処理
        if (other.TryGetComponent<ObjectType>(out var ObjectType))
        {
            switch (ObjectType.GetObjectType)
            {
                case Object_Type.Item:      //アイテム
                    ItemGet(other);
                    break;
                case Object_Type.Trap:      //トラップ
                    break;
                case Object_Type.obstacles: //障害物
                    break;
                case Object_Type.Player:    //プレイヤー
                    break;
                default:
                    break;
            }
        }
    }

    //接触したものがItemの場合の処理
    void ItemGet(Collider other)
    {
        if (other.TryGetComponent<ItemData>(out var Item_Data))
        {
            string ItemID = Item_Data.GetItem_ItemID;
            _PartsContainer.UpdateItemParts(ItemID);
            Destroy(other.gameObject);
        }
        else
        {
            UnityEngine.Debug.LogError("触れたアイテムはItemDataスクリプトがありません。");
        }
    }
}
